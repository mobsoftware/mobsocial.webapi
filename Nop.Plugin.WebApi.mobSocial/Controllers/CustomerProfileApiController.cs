using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Mob.Core;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Plugin.WebApi.MobSocial.Models;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Extensions;
using Nop.Plugin.WebApi.MobSocial.Services;
using Nop.Services.Common;
using Nop.Services.Media;
using Nop.Services.Customers;

namespace Nop.Plugin.WebApi.MobSocial.Controllers
{
    [RoutePrefix("api/customerprofile")]
    public class CustomerProfileApiController : BaseMobApiController
    {
        private readonly CustomerProfileService _customerProfileService;
        private readonly CustomerProfileViewService _customerProfileViewService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerFavoriteSongService _customerFavoriteSongService;
        private readonly IMobSocialService _mobSocialService;
        private readonly IWorkContext _workContext;
        private readonly IMusicService _musicService;
        private readonly IFriendService _friendService;
        private readonly IPictureService _pictureService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerFollowService _customerFollowService;
        private readonly MediaSettings _mediaSettings;
        private readonly mobSocialSettings _mobSocialSettings;

        public CustomerProfileApiController(CustomerProfileService customerProfileService,
            CustomerProfileViewService customerProfileViewService,
            ICustomerService customerService,
            IMobSocialService mobSocialService,
            ICustomerFavoriteSongService customerFavoriteSongService,
            IMusicService musicService,
            IWorkContext workContext, IFriendService friendService, IPictureService pictureService, mobSocialSettings mobSocialSettings, MediaSettings mediaSettings, IGenericAttributeService genericAttributeService, ICustomerFollowService customerFollowService)
        {
            _customerProfileService = customerProfileService;
            _customerProfileViewService = customerProfileViewService;
            _customerService = customerService;
            _customerFavoriteSongService = customerFavoriteSongService;
            _mobSocialService = mobSocialService;
            _musicService = musicService;
            _workContext = workContext;
            _friendService = friendService;
            _pictureService = pictureService;
            _mobSocialSettings = mobSocialSettings;
            _mediaSettings = mediaSettings;
            _genericAttributeService = genericAttributeService;
            _customerFollowService = customerFollowService;
        }



        [HttpPost]
        [Authorize]
        [Route("uploadpicture")]
        public IHttpActionResult UploadPicture()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            var files = HttpContext.Current.Request.Files;
         
             // check if files are on the request.*/
            if (files.Count == 0)
            {
                return Json(new { Success = false });
            }

            try
            {
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
                    //save the picture now
                    var picture = _pictureService.InsertPicture(pictureBytes, contentType, null);

                    newImages.Add(new {
                        ImageUrl = _pictureService.GetPictureUrl(picture.Id),
                        ImageId = picture.Id
                    });
                }

                return Json(new { Success = true, Images = newImages });
            }
            catch (Exception e)
            {
                return Json(new { Success = false });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("setpictureas/{uploadType}/{pictureId:int}")]
        public IHttpActionResult SetPictureAs(string uploadType, int pictureId)
        {
            /*Due to caching, generic attributes don't update the data somehow from apicontrollers*/
            //TODO: Find a workaround to this issue
            //for now we have created an extension method to bypass cache for retrieve. eventually this works for now
            string key = "";
            switch (uploadType)
            {
                case "cover":
                    key = AdditionalCustomerAttributeNames.CoverImageId;
                    break;
                case "avatar":
                    key = SystemCustomerAttributeNames.AvatarPictureId;
                    break;
            }
            //get the attribute with our extension method
            var attribute = _genericAttributeService.GetAttributeByKey(_workContext.CurrentCustomer, key);
            if (attribute == null)
            {
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, key, pictureId);
            }
            else
            {
                attribute.Value = pictureId.ToString();
                _genericAttributeService.UpdateAttribute(attribute);
            }
            return Json(new { Success = true });
        }

        [HttpPost]
        public void SaveCustomerProfile(CustomerProfileModel customerProfile)
        {

            var profile = _customerProfileService.GetByCustomerId(customerProfile.CustomerId);

            if (profile == null)
            {
                profile = new CustomerProfile() {
                    CustomerId = customerProfile.CustomerId,
                    AboutMe = customerProfile.AboutMe,
                    Website = customerProfile.Website,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                };

                _customerProfileService.Insert(profile);
                return;
            }
            else
            {
                profile.AboutMe = customerProfile.AboutMe;
                profile.Website = customerProfile.Website;
                profile.DateUpdated = DateTime.Now;
                _customerProfileService.Update(profile);
                return;
            }


        }


        [HttpPost]
        public void AddFavoriteSong(CustomerFavoriteSong favoriteSong)
        {
            var dateTimeNow = DateTime.Now;
            favoriteSong.DisplayOrder = 0;
            favoriteSong.DateCreated = dateTimeNow;
            favoriteSong.DateUpdated = dateTimeNow;

            _customerFavoriteSongService.Insert(favoriteSong);
        }

        [HttpPost]
        public void DeleteFavoriteSong(int id)
        {
            //_customerFavoriteSongService.LogicalDelete(id);
        }


        [HttpPost]
        public void UpdateFavoriteSongOrder(int favoriteSongId, int displayOrder)
        {
            _customerFavoriteSongService.UpdateFavoriteSongOrder(favoriteSongId, displayOrder);
        }



    }
}
