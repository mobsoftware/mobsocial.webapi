using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Http;
using Mob.Core;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Plugin.WebApi.MobSocial.Attributes;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Enums;
using Nop.Plugin.WebApi.MobSocial.Extensions.ModelExtensions;
using Nop.Plugin.WebApi.MobSocial.Models;
using Nop.Plugin.WebApi.MobSocial.Services;
using Nop.Services.Customers;
using Nop.Services.Media;

namespace Nop.Plugin.WebApi.MobSocial.Controllers
{
    [RoutePrefix("api/media")]
    public class MediaApiController : BaseMobApiController
    {
        private readonly IMediaService _mediaService;
        private readonly MediaSettings _mediaSettings;
        private readonly IMobSocialVideoProcessor _videoProcessor;
        private readonly ICustomerService _userService;
        private readonly ICustomerCommentService _commentService;
        private readonly ICustomerLikeService _likeService;
        private readonly IEntityMediaService _entityMediaService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerProfileService _customerProfileService;
        private readonly ICustomerProfileViewService _customerProfileViewService;
        private readonly ICustomerLikeService _customerLikeService;
        private readonly ICustomerFollowService _customerFollowService;
        private readonly ICustomerCommentService _customerCommentService;
        private readonly IFriendService _friendService;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper;
        public MediaApiController(MediaService mediaService, MediaSettings mediaSettings, IMobSocialVideoProcessor videoProcessor, ICustomerService userService, ICustomerCommentService commentService, ICustomerLikeService likeService, IEntityMediaService entityMediaService, IWorkContext workContext, IStoreContext storeContext, ICustomerProfileService customerProfileService, ICustomerProfileViewService customerProfileViewService, ICustomerLikeService customerLikeService, ICustomerFollowService customerFollowService, ICustomerCommentService customerCommentService, IFriendService friendService, IPictureService pictureService, IWebHelper webHelper)
        {
            _mediaService = mediaService;
            _mediaSettings = mediaSettings;
            _videoProcessor = videoProcessor;
            _userService = userService;
            _commentService = commentService;
            _likeService = likeService;
            _entityMediaService = entityMediaService;
            _workContext = workContext;
            _storeContext = storeContext;
            _customerProfileService = customerProfileService;
            _customerProfileViewService = customerProfileViewService;
            _customerLikeService = customerLikeService;
            _customerFollowService = customerFollowService;
            _customerCommentService = customerCommentService;
            _friendService = friendService;
            _pictureService = pictureService;
            _webHelper = webHelper;
        }

        [HttpGet]
        [ApiAuthorize]
        [Route("get/{id:int}")]
        public IHttpActionResult Get(int id)
        {
            var media = _mediaService.GetById(id);
            if (media == null)
                return NotFound();

            var entityMedia = _entityMediaService.FirstOrDefault(x => x.MediaId == id);
            MediaReponseModel model = null;
            //todo: verify permissions to see if media can be viewed by logged in user
            switch (entityMedia.EntityName)
            {
                case "Skill":
                    model = media.ToModel<Skill>(entityMedia.EntityId, _mediaService, _mediaSettings, _workContext,
                        _storeContext, _userService,
                        _customerProfileService, _customerProfileViewService, _pictureService, Url, _webHelper, _friendService,
                        _commentService, _likeService, true, true, true, true);
                    break;
                case "UserSkill":
                    model = media.ToModel<UserSkill>(entityMedia.EntityId, _mediaService, _mediaSettings, _workContext,
                        _storeContext, _userService,
                        _customerProfileService, _customerProfileViewService, _pictureService, Url, _webHelper, _friendService,
                        _commentService, _likeService, true, true, true, true);
                    break;
                default:
                    model = media.ToModel(_mediaService, _mediaSettings, _workContext, _storeContext, _userService,
                   _customerProfileService, _customerProfileViewService, _customerFollowService, _friendService,
                   _commentService, _likeService, _pictureService, Url, _webHelper, true, true, true);
                    break;
            }

            return Response(new { Success = true, Media = model });
        }

        [ApiAuthorize]
        [Route("uploadpictures")]
        [HttpPost]
        public IHttpActionResult UploadPictures()
        {
            var request = HttpContext.Current.Request;
            var files = request.Files;
            if (files.Count == 0)
            {
                return Response(new { Success = false });
            }
            //a little hack here.//if we pass model as paramter to this method we get -> This method or property is not supported after HttpRequest.GetBufferlessInputStream has been invoked
            PictureCropModel cropModel = null;
            if (request.Params["crop"] != null)
            {
                int left, width, top, height;
                int.TryParse(request.Params["left"] ?? "0", out left);
                int.TryParse(request.Params["width"] ?? "0", out width);
                int.TryParse(request.Params["top"] ?? "0", out top);
                int.TryParse(request.Params["height"] ?? "0", out height);
                cropModel = new PictureCropModel()
                {
                    Width = width,
                    Height = height,
                    Left = left,
                    Top = top
                };
            }
            var newImages = new List<object>();
            for (var index = 0; index < files.Count; index++)
            {

                //the file
                var file = files[index];

                //and it's name
                var fileName = file.FileName;
                //stream to read the bytes
                var stream = file.InputStream;
                var pictureBytes = new byte[stream.Length];
                stream.Read(pictureBytes, 0, pictureBytes.Length);

                //file extension and it's type
                var fileExtension = Path.GetExtension(fileName);
                if (!string.IsNullOrEmpty(fileExtension))
                    fileExtension = fileExtension.ToLowerInvariant();

                var contentType = file.ContentType;

                if (string.IsNullOrEmpty(contentType))
                {
                    contentType = PictureUtility.GetContentType(fileExtension);
                }

                var picture = new Media() {
                    Binary = pictureBytes,
                    MimeType = contentType,
                    Name = fileName,
                    UserId = _workContext.CurrentCustomer.Id,
                    DateCreated = DateTime.UtcNow
                };
                //should we crop or not
                if (cropModel?.Height > 0 && cropModel.Width > 0)
                {
                    _mediaService.WritePictureBytesCropped(picture, cropModel.Left, cropModel.Top, cropModel.Width,
                        cropModel.Height);
                }
                else
                {
                    _mediaService.WritePictureBytes(picture);
                }
                //save it
                _mediaService.Insert(picture);
                newImages.Add(picture.ToModel(_mediaService, _mediaSettings, _workContext, _storeContext, _userService,
                    _customerProfileService, _customerProfileViewService, _customerFollowService, _friendService,
                    _commentService, _likeService, _pictureService, Url, _webHelper));
            }

            return Response(new { Success = true, Images = newImages });
        }

        [ApiAuthorize]
        [Route("uploadvideo")]
        [HttpPost]
        public IHttpActionResult UploadVideo()
        {
            var files = HttpContext.Current.Request.Files;
            if (files.Count == 0)
            {
                return Response(new { Success = false });
            }

            var file = files[0];
            //and it's name
            var fileName = file.FileName;


            //file extension and it's type
            var fileExtension = Path.GetExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            var contentType = file.ContentType;

            if (string.IsNullOrEmpty(contentType))
            {
                contentType = VideoUtility.GetContentType(fileExtension);
            }

            if (contentType == string.Empty)
            {
                return Response(new { Success = false });
            }

            var bytes = new byte[file.ContentLength];
            file.InputStream.Read(bytes, 0, bytes.Length);

            //create a new media
            var media = new Media() {
                MediaType = MediaType.Video,
                Binary = bytes,
                MimeType = contentType,
                Name = fileName,
                UserId = _workContext.CurrentCustomer.Id,
                DateCreated = DateTime.UtcNow
            };

            _mediaService.WriteVideoBytes(media);
            //insert now
            _mediaService.Insert(media);
            return Response(new {
                Success = true,
                Video =
                media.ToModel(_mediaService, _mediaSettings, _workContext, _storeContext, _userService,
                    _customerProfileService, _customerProfileViewService, _customerFollowService, _friendService,
                    _commentService, _likeService, _pictureService, Url, _webHelper)
            });
        }
    }
}