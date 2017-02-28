using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mob.Core;
using Mob.Core.Data;
using Mob.Core.Extensions;
using Mob.Core.Services;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Plugin.WebApi.MobSocial.Constants;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Enums;
using Nop.Services.Media;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public class MediaService : BaseEntityService<Media>, IMediaService
    {
        private readonly IEntityMediaService _entityMediaService;
        private readonly MediaSettings _mediaSettings;
        private readonly IPictureService _pictureService;
        private readonly IMobSocialImageProcessor _imageProcessor;
        private readonly IStoreContext _storeContext;
        private readonly IMobSocialVideoProcessor _videoProcessor;
        public MediaService(IMobRepository<Media> mediaRepository, IEntityMediaService entityMediaService, MediaSettings mediaSettings, IPictureService pictureService, IMobSocialImageProcessor imageProcessor, IStoreContext storeContext, IMobSocialVideoProcessor videoProcessor) : base(mediaRepository)
        {
            _entityMediaService = entityMediaService;
            _mediaSettings = mediaSettings;
            _pictureService = pictureService;
            _imageProcessor = imageProcessor;
            _storeContext = storeContext;
            _videoProcessor = videoProcessor;
        }


        public void AttachMediaToEntity<T>(int entityId, int mediaId) where T : BaseEntity
        {
            if (mediaId == 0)
            {
                throw new Exception("Can't attach entity with media with Id '0'");
            }
           //insert entity picture only if it doesn't exist
            var insertRequired  =
                !_entityMediaService.Get(x => x.EntityId == entityId && x.MediaId == mediaId).Any();

            if (!insertRequired) return;


            var entityPicture = new EntityMedia()
            {
                EntityId = entityId,
                MediaId = mediaId,
                EntityName = typeof(T).Name
            };

            _entityMediaService.Insert(entityPicture);
        }

        public void AttachMediaToEntity<T>(T entity, Media media) where T : BaseEntity
        {
            if (media.Id == 0)
            {
                Repository.Insert(media);
            }
            AttachMediaToEntity<T>(entity.Id, media.Id);
        }

        public void DetachMediaFromEntity<T>(int entityId, int mediaId) where T : BaseEntity
        {
            _entityMediaService.Delete(x => x.EntityId == entityId && x.MediaId == mediaId);
        }

        public void DetachMediaFromEntity<T>(T entity, Media media) where T : BaseEntity
        {
            DetachMediaFromEntity<T>(entity.Id, media.Id);
        }

        public void ClearMediaAttachments(Media media)
        {
            _entityMediaService.Delete(x => x.MediaId == media.Id);
        }

        public void ClearEntityMedia<T>(int entityId) where T : BaseEntity
        {
            _entityMediaService.Delete(x => x.EntityId == entityId);
        }

        public void ClearEntityMedia<T>(T entity) where T : BaseEntity
        {
            ClearEntityMedia<T>(entity.Id);
        }

        public string GetPictureUrl(int pictureId, int width = 0, int height = 0, bool returnDefaultIfNotFound = false)
        {
            var picture = Repository.GetById(pictureId);
            return GetPictureUrl(picture, width, height, returnDefaultIfNotFound);
        }

        public string GetPictureUrl(Media picture, int width = 0, int height = 0, bool returnDefaultIfNotFound = false)
        {
            //check if picture is not null
            if (picture == null || picture.Id == 0)
                return string.Empty;

            var expectedFile = picture.LocalPath;
            var expectedFileSystemPath = ServerHelper.GetLocalPathFromRelativePath(expectedFile);

            if (!File.Exists(expectedFileSystemPath))
            {
                //we need to create the file with required dimensions
                var fileSystemPathForOriginalImage = ServerHelper.GetLocalPathFromRelativePath(picture.LocalPath);

                //image format
                var imageFormat = PictureUtility.GetImageFormatFromContentType(picture.MimeType);
                //save resized image
                _imageProcessor.WriteResizedImage(fileSystemPathForOriginalImage, expectedFileSystemPath, width, height, imageFormat);
            }
            //return the image url
            var imageServeUrl = expectedFile.Replace("~", _storeContext.CurrentStore.Url);
            return imageServeUrl;
        }

        public string GetVideoUrl(int mediaId)
        {
            var media = GetById(mediaId);
            return GetVideoUrl(media);
        }

        public string GetVideoUrl(Media media)
        {
            //check if picture is not null
            if (media == null || media.Id == 0)
                return string.Empty;

            var expectedFile = media.LocalPath;
            
            //return the image url
            var videoServeUrl = expectedFile.Replace("~", _storeContext.CurrentStore.Url);
            return videoServeUrl;
        }

        public void WritePictureBytes(Media picture)
        {
            //we need to save the file on file system
            if (picture.Binary == null || !picture.Binary.Any())
            {
                throw new Exception("Can't write empty bytes for picture");
            }

            //get the directory path from the relative path
            var directoryPath = ServerHelper.GetLocalPathFromRelativePath(MobSocialConstant.MobSocialPicturePath);
            var fileExtension = PathUtility.GetFileExtensionFromContentType(picture.MimeType);

            if (string.IsNullOrEmpty(picture.SystemName))
                picture.SystemName = $"{Guid.NewGuid().ToString("n")}";

            var proposedFileName = $"{picture.SystemName}{fileExtension}";
            var filePath = PathUtility.GetFileSavePath(directoryPath, proposedFileName);

            var imageFormat = PictureUtility.GetImageFormatFromContentType(picture.MimeType);
            _imageProcessor.WriteBytesToImage(picture.Binary, filePath, imageFormat);

            //clear bytes
            picture.Binary = null;
            picture.LocalPath = ServerHelper.GetRelativePathFromLocalPath(filePath);

            picture.ThumbnailPath = ServerHelper.GetRelativePathFromLocalPath(filePath);
        }

        public void WriteVideoBytes(Media video)
        {
            //we need to save the file on file system
            if (video.Binary == null || !video.Binary.Any())
            {
                throw new Exception("Can't write empty bytes for picture");
            }

            //get the directory path from the relative path
            var directoryPath = ServerHelper.GetLocalPathFromRelativePath(MobSocialConstant.MobSocialVideoPath);
            var fileExtension = PathUtility.GetFileExtensionFromContentType(video.MimeType);

            if (string.IsNullOrEmpty(video.SystemName))
                video.SystemName = $"{Guid.NewGuid().ToString("n")}";

            var proposedFileName = $"{video.SystemName}{fileExtension}";
            var filePath = PathUtility.GetFileSavePath(directoryPath, proposedFileName);
            File.WriteAllBytes(filePath, video.Binary);

            //clear bytes
            video.Binary = null;
            video.LocalPath = ServerHelper.GetRelativePathFromLocalPath(filePath);

            //create thumbnail for video
            var thumbnailRelativeFilePath = Path.Combine(MobSocialConstant.MobSocialPicturePath, video.SystemName + ".thumb.jpg");
            var thumbnailLocalFilePath = ServerHelper.GetLocalPathFromRelativePath(thumbnailRelativeFilePath);
            //TODO: Generate thumbnails of different sizes to save bandwidth

            _videoProcessor.WriteVideoThumbnailPicture(filePath, thumbnailLocalFilePath);
            //store relative path in thumbnail path
            video.ThumbnailPath = thumbnailRelativeFilePath;
        }

        public IQueryable<Media> GetEntityMedia<TEntityType>(int entityId, MediaType? mediaType, int page = 1, int count = 15) where TEntityType : BaseEntity
        {
            //first get the media ids for this entity
            var mediaIds = _entityMediaService.Get(x => x.EntityId == entityId && x.EntityName == typeof(TEntityType).Name).Select(x => x.MediaId).ToList();
            if (mediaType.HasValue)
                return Get(x => mediaIds.Contains(x.Id) && x.MediaType == mediaType);
            return Get(x => mediaIds.Contains(x.Id));
        }

        public override List<Media> GetAll(string Term, int Count = 15, int Page = 1)
        {
            throw new NotImplementedException();
        }
    }
}