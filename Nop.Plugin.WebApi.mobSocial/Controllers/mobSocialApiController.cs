using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Seo;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Mob.Core;
using System.Web.Http;
using System.Web.Routing;
using Nop.Plugin.WebApi.MobSocial.Services;
using Nop.Plugin.WebApi.MobSocial.Extensions;
using SeoExtensions = Nop.Plugin.WebApi.MobSocial.Extensions.SeoExtensions;

namespace Nop.Plugin.WebApi.MobSocial.Controllers
{
    [RoutePrefix("api/mobsocial")]
    public class mobSocialApiController : BaseMobApiController
    {

        private IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private AdminAreaSettings _adminAreaSettings;
        private ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly mobSocialSettings _mobSocialSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly OrderService _orderService;
        private readonly ForumSettings _forumSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly OrderSettings _orderSettings;
        private readonly IStoreContext _storeContext;
        private readonly IMobSocialService _socialNetworkService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerAlbumPictureService _customerAlbumPictureService;
        private IWebHelper _webHelper;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IRepository<UrlRecord> _urlRecordRepository;
        private readonly ICustomerVideoAlbumService _customerVideoAlbumService;
        private readonly CustomerProfileViewService _customerProfileViewService;

        public mobSocialApiController(IPermissionService permissionService,
            IWorkContext workContext, AdminAreaSettings adminAreaSettings, ILocalizationService localizationService,
            IPictureService pictureService, IMobSocialService socialNetworkService, ICustomerService customerService,
            ICustomerAlbumPictureService customerAlbumPictureService, mobSocialSettings mobSocialSettings, MediaSettings mediaSettings, CustomerSettings customerSettings, 
            ForumSettings forumSettings, RewardPointsSettings rewardPointsSettings, OrderSettings orderSettings,
             IStoreContext storeContext, IWebHelper webHelper, IUrlRecordService urlRecordService, IRepository<UrlRecord> urlRecordRepository,
            ICustomerVideoAlbumService customerVideoAlbumService, CustomerProfileViewService customerProfileViewService)
        {
            _permissionService = permissionService;
            _workContext = workContext;
            _adminAreaSettings = adminAreaSettings;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _socialNetworkService = socialNetworkService;
            _customerService = customerService;
            _customerAlbumPictureService = customerAlbumPictureService;
            _mobSocialSettings = mobSocialSettings;
            _mediaSettings = mediaSettings;
            _customerSettings = customerSettings;
            _forumSettings = forumSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _orderSettings = orderSettings;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _urlRecordService = urlRecordService;
            _urlRecordRepository = urlRecordRepository;
            _customerVideoAlbumService = customerVideoAlbumService;
            _customerProfileViewService = customerProfileViewService;
        }

        
        public IHttpActionResult ConfirmFriend(int friendCustomerId)
        {
            try
            {
                _socialNetworkService.ConfirmFriendRequest(friendCustomerId);
                return Json(new { success = true, message = "Friend Confirmed!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Could not add friend. Please try again. If the problem persists, please contact us." });
            }
            
        }


        [Route("searchtermautocomplete")]
        [HttpGet]
        public IHttpActionResult SearchTermAutoComplete(string term = "", bool excludeLoggedInUser = true)
        {
            if (String.IsNullOrWhiteSpace(term) || term.Length < _mobSocialSettings.PeopleSearchTermMinimumLength)
                return Json(new object());

            _mobSocialSettings.PeopleSearchAutoCompleteNumberOfResults = 10;

            //TODO: Find a better way to implement this search
            
            //a search term may be first name or last name...nopcommerce puts an 'and' filter rather than an 'or' filter.
            //we therefore need to first get all the customers and then filter them according to name

            var customerRole = _customerService.GetCustomerRoleBySystemName("Registered");
            var customerRoleIds = new int[1];
            if (customerRole != null)
                customerRoleIds[0] = customerRole.Id;

            var customers = _customerService.GetAllCustomers(null, null, 0, 0, customerRoleIds).ToList();

            customers = excludeLoggedInUser ? customers.Where(x => x.Id != _workContext.CurrentCustomer.Id).ToList() : customers;
            var count = _mobSocialSettings.PeopleSearchAutoCompleteNumberOfResults;
            term = term.ToLowerInvariant();
            var filteredCustomers = new List<Customer>();
            customers.ForEach(x =>
            {

                if (filteredCustomers.Count >= _mobSocialSettings.PeopleSearchAutoCompleteNumberOfResults) return;
                
                var firstName = x.GetAttribute<string>(SystemCustomerAttributeNames.FirstName);
                firstName = firstName == null ? "" : firstName.ToLowerInvariant();
                var lastName = x.GetAttribute<string>(SystemCustomerAttributeNames.LastName);
                lastName = lastName == null ? "" : lastName.ToLowerInvariant();
                var email = x.Email;
                if (!firstName.Contains(term) && !lastName.Contains(term) && email != term) return;
                
                count--;
                filteredCustomers.Add(x);
            });
           
           
            var models = new List<object>();

            foreach (var c in filteredCustomers)
            {

                models.Add(new
                    {
                        DisplayName = c.GetFullName(),
                        PictureUrl = _pictureService.GetPictureUrl(
                            c.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId), 50),

                        ProfileUrl = Url.RouteUrl("CustomerProfileUrl", new RouteValueDictionary()
                        {
                             { "SeName" , SeoExtensions.GetSeName(c, 0) }
                        }),
                        Id = c.Id
                    });


            }

            return Json(models);

        }


        /// <summary>
        /// Saves a customer's video to the specified album
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [HandleJsonError]
        public void SaveVideo(int videoAlbumId, string videoUrl)
        {

            var videoAlbum = _customerVideoAlbumService.GetCustomerVideoAlbumById(videoAlbumId);


            if (videoAlbum.Videos.Count >= _mobSocialSettings.MaximumMainAlbumVideos)
                throw new ApplicationException("You may only upload up to " + _mobSocialSettings.MaximumMainAlbumVideos +
                                               " videos at this time.");


            var video = new CustomerVideo()
                {
                    VideoAlbum = videoAlbum,
                    CustomerVideoAlbumId = videoAlbumId,
                    DateCreated = DateTime.Now,
                    DisplayOrder = 0,
                    VideoUrl = videoUrl
                };

            _customerVideoAlbumService.Insert(video);


        }



        /// <summary>
        /// Adds a like to a customer's video
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [HandleJsonError]
        public IHttpActionResult AddVideoLike(int customerVideoId)
        {


            if (_workContext.CurrentCustomer.IsGuest())
                return Json(new {redirect = Url.RouteUrl("Login", null)});

            
            _customerVideoAlbumService.AddVideoLike(customerVideoId, _workContext.CurrentCustomer.Id);



            return Json(new {Success = true});

        }



        /*
         * NOTE: We are not providing the ablity to dislike a video already liked, unless
         * there is good psychology or other justification. Accidental like is not frequent
         * enough to justify unliking a video. Furthermore, unliking videos can artificially
         * demote customer videos and we need to prevent malicious demotions. - Bruce Leggett
         * /// <summary>
        /// Removes a like from a customer's video
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [HandleJsonError]
        public void RemoveVideoLike(int customerVideoId)
        {
            var video = _customerVideoAlbumService.GetCustomerVideoById(customerVideoId);
            video.LikeCount--;
            _customerVideoAlbumService.Update(video);
        }*/


        /// <summary>
        /// Deletes the customer album video matching the given id.
        /// </summary>
        /// <param name="customerVideoId">Id of customer video to delete</param>
        [HttpPost]
        public void DeleteCustomerVideo(int customerVideoId)
        {

            _customerVideoAlbumService.DeleteCustomerVideo(customerVideoId);

            //TODO: Later add ability to upload videos to server (Enabled by default but can be disabled in the mobSocial Admin) 


        }




        /// <summary>
        /// Deletes the customer album picture matching the given id.
        /// </summary>
        /// <param name="customerAlbumPictureId">Id of customer album picture to delete</param>
        [HttpPost]
        public void DeleteCustomerAlbumPicture(int customerAlbumPictureId)
        {
            var picture = _customerAlbumPictureService.GetCustomerAlbumPictureById(customerAlbumPictureId);

            var picturePath = CommonHelper.MapPath("~/" + picture.Url);
            var pictureFileName = Path.GetFileName(picturePath);

            var thumbPath = CommonHelper.MapPath("~/" + picture.ThumbnailUrl);
            var thumbFileName = Path.GetFileName(thumbPath);

            string deletedAlbumFolder = string.Format("~/Content/Images/Albums/{0}/{1}/Deleted",
                                                      picture.Album.CustomerId, picture.CustomerAlbumId);

            deletedAlbumFolder = CommonHelper.MapPath(deletedAlbumFolder);

            var deletedDirectory = new DirectoryInfo(deletedAlbumFolder);
            deletedDirectory.Create(); // If the directory already exists, nothing will happen here.

            
            // copy picture and thumb to deleted folder for archiving and historical reasons.
            FileUtility.MoveAndAddNumberIfExists(picturePath, deletedAlbumFolder);
            FileUtility.MoveAndAddNumberIfExists(thumbPath, deletedAlbumFolder);

            _customerAlbumPictureService.Delete(picture);

        }
       

        private void CreateSampleData()
        {
            var teamPage = new TeamPage()
            {
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
                UpdatedBy = 1,
                UpdatedOn = DateTime.Now,
                Name = "SkateMob",
                Description = "SkateMob members are the #1 skaters in the world!",
                TeamPictureId = 0

            };


            _socialNetworkService.InsertTeamPage(teamPage);


            var groupPage = new GroupPage()
            {
                Name = "Soldiers",
                Description = "New and upcoming skaters",
                PayPalDonateUrl = ""

            };

            _socialNetworkService.InsertGroupPage(groupPage);


        }

      
     


    }





}
