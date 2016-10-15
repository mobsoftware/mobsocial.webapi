using System.Web.Mvc;
using AutoMapper;
using Nop.Admin.Controllers;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Plugin.WebApi.MobSocial.Constants;
using Nop.Plugin.WebApi.MobSocial.Models;
using Nop.Plugin.WebApi.MobSocial.Services;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Controllers;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.WebApi.MobSocial.Controllers
{
    public class ConfigurationController : BasePublicController
    {
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ISkillService _skillService;

        public ConfigurationController(IPermissionService permissionService,
            ISettingService settingService,
            IStoreService storeService, IWorkContext workContext, ISkillService skillService)
        {
            _permissionService = permissionService;
            _settingService = settingService;
            _storeService = storeService;
            _workContext = workContext;
            _skillService = skillService;

            Mapper.Initialize(cfg => cfg.CreateMap<ConfigurationModel, mobSocialSettings>());
        }

        [AdminAuthorize]
        public ActionResult Configure(bool isSubmit = false)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);

            var settings = _settingService.LoadSetting<mobSocialSettings>(storeScope);
            var model = new ConfigurationModel();

            Mapper.Initialize(cfg => cfg.CreateMap<mobSocialSettings, ConfigurationModel>());
            Mapper.Map(settings, model);

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.ArtistPageMainImageSize_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.ArtistPageThumbnailSize_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.BattleHostSponsorshipPercentage_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.BattleVoteReminderEmailThresholdDays_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.CustomerAlbumPictureThumbnailWidth_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.CustomerProfileUpdateViewCountAfterNumberOfSeconds_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.DefaultVotingChargeForPaidVoting_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.EnableAutomaticMigrations_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.EchonestAPIKey_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.EventPageSearchAutoCompleteNumberOfResults_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.EventPageSearchTermMinimumLength_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.EventPageAttendanceThumbnailSize_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.FacebookWebsiteAppId_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.SevenDigitalPartnerId_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.ShowProfileImagesInSearchAutoComplete_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.SongFileMaximumUploadSize_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.SiteOwnerSponsorshipPercentage_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.SongFileSampleMaximumUploadSize_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.MacroPaymentsFixedPaymentProcessingFee_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.MicroPaymentsFixedPaymentProcessingFee_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.MacroPaymentsPaymentProcessingPercentage_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.MicroPaymentsPaymentProcessingPercentage_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.SiteOwnerSponsorshipPercentage_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.EnableFacebookInvite_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.VideoUploadReminderEmailThresholdDays_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.PeopleSearchAutoCompleteEnabled_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.PurchasedSongFeePercentage_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.ProfilePictureSize_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.TimelineSmallImageWidth_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);
                model.UninvitedFriendsNumberOfResults_OverrideForStore = _settingService.SettingExists(settings, x => x.ArtistPageMainImageSize, storeScope);

            }


            if (isSubmit)
            {
                model.MessageText = "Settings Saved Successfully.";
            }


            return View("~/Plugins" + (MobSocialConstant.SuiteInstallation ? "/MobSocial.Suite" : "/WebApi.mobSocial") + "/Views/Configuration/Configuration.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var settings = _settingService.LoadSetting<mobSocialSettings>(storeScope);

            //store the settings
            Mapper.Map(model, settings);


            if (model.ArtistPageMainImageSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.ArtistPageMainImageSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.ArtistPageMainImageSize, storeScope);

            if (model.ArtistPageThumbnailSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.ArtistPageThumbnailSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.ArtistPageThumbnailSize, storeScope);

            if (model.BattleHostSponsorshipPercentage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.BattleHostSponsorshipPercentage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.BattleHostSponsorshipPercentage, storeScope);

            if (model.BattleVoteReminderEmailThresholdDays_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.BattleVoteReminderEmailThresholdDays, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.BattleVoteReminderEmailThresholdDays, storeScope);

            if (model.CustomerAlbumPictureThumbnailWidth_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.CustomerAlbumPictureThumbnailWidth, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.CustomerAlbumPictureThumbnailWidth, storeScope);

            if (model.CustomerProfileUpdateViewCountAfterNumberOfSeconds_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.CustomerProfileUpdateViewCountAfterNumberOfSeconds, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.CustomerProfileUpdateViewCountAfterNumberOfSeconds, storeScope);

            if (model.DefaultVotingChargeForPaidVoting_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.DefaultVotingChargeForPaidVoting, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.DefaultVotingChargeForPaidVoting, storeScope);

            if (model.EnableAutomaticMigrations_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.EnableAutomaticMigrations, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.EnableAutomaticMigrations, storeScope);

            if (model.EchonestAPIKey_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.EchonestAPIKey, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.EchonestAPIKey, storeScope);

            if (model.EventPageSearchAutoCompleteNumberOfResults_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.EventPageSearchAutoCompleteNumberOfResults, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.EventPageSearchAutoCompleteNumberOfResults, storeScope);

            if (model.EventPageSearchTermMinimumLength_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.EventPageSearchTermMinimumLength, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.EventPageSearchTermMinimumLength, storeScope);

            if (model.EventPageAttendanceThumbnailSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.EventPageAttendanceThumbnailSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.EventPageAttendanceThumbnailSize, storeScope);

            if (model.FacebookWebsiteAppId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.FacebookWebsiteAppId, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.FacebookWebsiteAppId, storeScope);

            if (model.SevenDigitalPartnerId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.SevenDigitalPartnerId, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.SevenDigitalPartnerId, storeScope);

            if (model.ShowProfileImagesInSearchAutoComplete_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.ShowProfileImagesInSearchAutoComplete, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.ShowProfileImagesInSearchAutoComplete, storeScope);

            if (model.SongFileMaximumUploadSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.SongFileMaximumUploadSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.SongFileMaximumUploadSize, storeScope);

            if (model.SiteOwnerSponsorshipPercentage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.SiteOwnerSponsorshipPercentage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.SiteOwnerSponsorshipPercentage, storeScope);

            if (model.SongFileSampleMaximumUploadSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.SongFileSampleMaximumUploadSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.SongFileSampleMaximumUploadSize, storeScope);

            if (model.MacroPaymentsFixedPaymentProcessingFee_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.MacroPaymentsFixedPaymentProcessingFee, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.MacroPaymentsFixedPaymentProcessingFee, storeScope);

            if (model.MicroPaymentsFixedPaymentProcessingFee_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.MicroPaymentsFixedPaymentProcessingFee, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.MicroPaymentsFixedPaymentProcessingFee, storeScope);

            if (model.MacroPaymentsPaymentProcessingPercentage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.MacroPaymentsPaymentProcessingPercentage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.MacroPaymentsPaymentProcessingPercentage, storeScope);

            if (model.MicroPaymentsPaymentProcessingPercentage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.MicroPaymentsPaymentProcessingPercentage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.MicroPaymentsPaymentProcessingPercentage, storeScope);

            if (model.SiteOwnerSponsorshipPercentage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.SiteOwnerSponsorshipPercentage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.SiteOwnerSponsorshipPercentage, storeScope);

            if (model.EnableFacebookInvite_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.EnableFacebookInvite, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.EnableFacebookInvite, storeScope);

            if (model.VideoUploadReminderEmailThresholdDays_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.VideoUploadReminderEmailThresholdDays, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.VideoUploadReminderEmailThresholdDays, storeScope);

            if (model.PeopleSearchAutoCompleteEnabled_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.PeopleSearchAutoCompleteEnabled, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.PeopleSearchAutoCompleteEnabled, storeScope);
            
            if (model.PurchasedSongFeePercentage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.PurchasedSongFeePercentage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.PurchasedSongFeePercentage, storeScope);

            if (model.ProfilePictureSize_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.ProfilePictureSize, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.ProfilePictureSize, storeScope);

            if (model.TimelineSmallImageWidth_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.TimelineSmallImageWidth, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.TimelineSmallImageWidth, storeScope);

            if (model.UninvitedFriendsNumberOfResults_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.UninvitedFriendsNumberOfResults, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.UninvitedFriendsNumberOfResults, storeScope);


            //now clear settings cache
            _settingService.ClearCache();

            return Configure(true);
        }

        [AdminAuthorize]
        public ActionResult ConfigureSkills(int page = 1, int count = 15)
        {
            return View("~/Plugins" + (MobSocialConstant.SuiteInstallation ? "/MobSocial.Suite" : "/WebApi.mobSocial") + "/Views/Configuration/SkillConfiguration.cshtml");
        }
    }
}