using System.Linq;
using Mob.Core.Services;
using Nop.Core;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Enums;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface IMediaService : IBaseEntityService<Media>
    {
       void AttachMediaToEntity<T>(int entityId, int mediaId) where T : BaseEntity;

        void AttachMediaToEntity<T>(T entity, Media media) where T : BaseEntity;

        void DetachMediaFromEntity<T>(int entityId, int mediaId) where T : BaseEntity;

        void DetachMediaFromEntity<T>(T entity, Media media) where T : BaseEntity;

        void ClearEntityMedia<T>(int entityId) where T : BaseEntity;

        void ClearEntityMedia<T>(T entity) where T : BaseEntity;

        void ClearMediaAttachments(Media media);

        string GetPictureUrl(int pictureId, int width = 0, int height = 0, bool returnDefaultIfNotFound = false);

        string GetPictureUrl(Media picture, int width = 0, int height = 0, bool returnDefaultIfNotFound = false);

        string GetVideoUrl(int mediaId);

        string GetVideoUrl(Media media);

        void WritePictureBytes(Media picture);

        void WriteVideoBytes(Media video);

        IQueryable<Media> GetEntityMedia<TEntityType>(int entityId, MediaType? mediaType, int page = 1, int count = 15) where TEntityType : BaseEntity;
    }
}