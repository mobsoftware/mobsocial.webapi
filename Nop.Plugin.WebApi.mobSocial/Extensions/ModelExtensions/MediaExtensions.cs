using System;
using System.Linq;
using System.Web.Http.Routing;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Enums;
using Nop.Plugin.WebApi.MobSocial.Models;
using Nop.Plugin.WebApi.MobSocial.Services;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Media;

namespace Nop.Plugin.WebApi.MobSocial.Extensions.ModelExtensions
{
    public static class MediaExtensions
    {
        public static MediaReponseModel ToModel(this Media media, 
            IMediaService mediaService, 
            MediaSettings mediaSettings, 
            IWorkContext workContext,
            IStoreContext storeContext,
            ICustomerService userService, 
            ICustomerProfileService customerProfileService,
            ICustomerProfileViewService customerProfileViewService,
            ICustomerFollowService followService,
            IFriendService friendService,
            ICustomerCommentService commentService,
            ICustomerLikeService likeService,
            IPictureService pictureService,
            UrlHelper urlHelper,
            IWebHelper webHelper,
            bool withUserInfo = true,
            bool withSocialInfo = false,
            bool withNextAndPreviousMedia = false)
        {
            var storeUrl = webHelper.GetStoreLocation();
            var model = new MediaReponseModel()
            {
                Id = media.Id,
                MediaType = media.MediaType,
                Url =
                    media.MediaType == MediaType.Image
                        ? mediaService.GetPictureUrl(media)
                        : mediaService.GetVideoUrl(media),
                MimeType = media.MimeType,
                DateCreatedUtc = media.DateCreated,
                ThumbnailUrl =
                    media.MediaType == MediaType.Image
                        ? mediaService.GetPictureUrl(media)
                        : media.ThumbnailPath.Replace("~", storeUrl)
            };
            if (withUserInfo && userService != null)
            {
                var user = userService.GetCustomerById(media.UserId);
                if (user != null)
                {
                    var dateTimeHelper = EngineContext.Current.Resolve<IDateTimeHelper>();
                    model.CreatedBy = user.ToPublicModel(workContext, customerProfileViewService, customerProfileService, pictureService, mediaSettings, urlHelper);
                    model.DateCreatedLocal = dateTimeHelper.ConvertToUserTime(media.DateCreated, DateTimeKind.Utc);
                }
            }
            if (withSocialInfo)
            {
                if (likeService != null)
                {
                    model.TotalLikes = likeService.GetLikeCount<Media>(media.Id);
                    model.LikeStatus =
                        likeService.GetCustomerLike<Media>(workContext.CurrentCustomer.Id, media.Id) != null
                            ? 1
                            : 0;
                }

                if (commentService != null)
                {
                    model.TotalComments = commentService.GetCommentsCount(media.Id, typeof(Media).Name);
                    model.CanComment = true; //todo: perform check if comments are enabled or user has permission to comment
                }
            }

            if (withNextAndPreviousMedia)
            {
                var allMedia = mediaService.GetEntityMedia<Customer>(media.UserId, media.MediaType, 1, int.MaxValue).ToList();
                var mediaIndex = allMedia.FindIndex(x => x.Id == media.Id);

                model.PreviousMediaId = mediaIndex <= 0 ? 0 : allMedia[mediaIndex - 1].Id;
                model.NextMediaId = mediaIndex < 0 || mediaIndex == allMedia.Count - 1 ? 0 : allMedia[mediaIndex + 1].Id;
            }

            model.FullyLoaded = withSocialInfo && withNextAndPreviousMedia;
            return model;
            ;
        }

        public static MediaReponseModel ToModel<T>(this Media media, int entityId,
            IMediaService mediaService,
            MediaSettings mediaSettings,
            IWorkContext workContext,
            IStoreContext storeContext,
            ICustomerService userService,
            ICustomerProfileService customerProfileService,
            ICustomerProfileViewService customerProfileViewService,
            IPictureService pictureService,
            UrlHelper urlHelper,
            IWebHelper webHelper,
            IFriendService friendService = null,
            ICustomerCommentService commentService = null,
            ICustomerLikeService likeService = null,
            bool withUserInfo = true,
            bool withSocialInfo = false,
            bool withNextAndPreviousMedia = false,
            bool avoidMediaTypeForNextAndPreviousMedia = false) where T: BaseEntity
        {
            var storeUrl = webHelper.GetStoreLocation();
            var model = new MediaReponseModel() {
                Id = media.Id,
                MediaType = media.MediaType,
                Url = media.MediaType == MediaType.Image ? mediaService.GetPictureUrl(media) : mediaService.GetVideoUrl(media),
                MimeType = media.MimeType,
                DateCreatedUtc = media.DateCreated,
                ThumbnailUrl = media.MediaType == MediaType.Image ? mediaService.GetPictureUrl(media) : media.ThumbnailPath.Replace("~", storeUrl)
            };
            if (withUserInfo && userService != null)
            {
                var user = userService.GetCustomerById(media.UserId);
                if (user != null)
                {
                    var dateTimeHelper = EngineContext.Current.Resolve<IDateTimeHelper>();
                    model.CreatedBy = user.ToPublicModel(workContext, customerProfileViewService, customerProfileService, pictureService, mediaSettings, urlHelper);
                    model.DateCreatedLocal = dateTimeHelper.ConvertToUserTime(media.DateCreated, DateTimeKind.Utc);
                }
            }
            if (withSocialInfo)
            {
                if (likeService != null)
                {
                    model.TotalLikes = likeService.GetLikeCount<Media>(media.Id);
                    model.LikeStatus =
                        likeService.GetCustomerLike<Media>(workContext.CurrentCustomer.Id, media.Id) != null
                            ? 1
                            : 0;
                }

                if (commentService != null)
                {
                    model.TotalComments = commentService.GetCommentsCount(media.Id, typeof(Media).Name);
                    model.CanComment = true; //todo: perform check if comments are enabled or user has permission to comment
                }
            }

            if (withNextAndPreviousMedia)
            {
                MediaType? mediaType = null;
                if (!avoidMediaTypeForNextAndPreviousMedia)
                    mediaType = media.MediaType;
                var allMedia = mediaService.GetEntityMedia<T>(entityId, mediaType, 1, int.MaxValue).ToList();
                var mediaIndex = allMedia.FindIndex(x => x.Id == media.Id);

                model.PreviousMediaId = mediaIndex <= 0 ? 0 : allMedia[mediaIndex - 1].Id;
                model.NextMediaId = mediaIndex < 0 || mediaIndex == allMedia.Count - 1 ? 0 : allMedia[mediaIndex + 1].Id;
            }

            model.FullyLoaded = withSocialInfo && withNextAndPreviousMedia;
            return model;
            ;
        }
    }
}