using System.Collections.Generic;
using System.Web.Routing;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Tasks;
using Nop.Core.Plugins;
using Nop.Plugin.WebApi.MobSocial.Data;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Tasks;
using Nop.Web.Framework.Menu;
using System.Web.Configuration;
using Nop.Core;
using System.Linq;
using Nop.Plugin.WebApi.MobSocial.Constants;
using Nop.Plugin.WebApi.MobSocial.Services;

namespace Nop.Plugin.WebApi.MobSocial
{
    public class MobSocialWebApiPlugin : BasePlugin, IAdminMenuPlugin
    {


        private readonly MobSocialObjectContext _context;
        private readonly mobSocialSettings _mobSocialSettings;
        private readonly ISettingService _settingService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IMobSocialService _mobSocialService;
        private readonly ILocalizationService _localizationService;
        private readonly HttpRuntimeSection _config;
        private readonly IStoreContext _storeContext;
        private readonly IVideoBattleService _videoBattleService;
        private readonly EmailAccountSettings _emailAccountSettings;

        public MobSocialWebApiPlugin(MobSocialObjectContext context, mobSocialSettings mobSocialSettings,
            ISettingService settingService, IMessageTemplateService messageTemplateService,
            IScheduleTaskService scheduleTaskService,
            IMobSocialService mobSocialService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IVideoBattleService videoBattleService, EmailAccountSettings emailAccountSettings)
        {
            _context = context;
            _mobSocialSettings = mobSocialSettings;
            _settingService = settingService;
            _messageTemplateService = messageTemplateService;
            _scheduleTaskService = scheduleTaskService;
            _mobSocialService = mobSocialService;
            _localizationService = localizationService;
            _videoBattleService = videoBattleService;
            _emailAccountSettings = emailAccountSettings;
            _storeContext = storeContext;
            _config = new HttpRuntimeSection(); //TODO Move to dependency registrar and perform injection

        }


        #region Methods

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //locales
            //this.AddOrUpdatePluginLocaleResource("Plugins.ProductCarousel.Cache", "true");
            //It's required to set initializer to null (for SQL Server Compact).
            //otherwise, you'll get something like "The model backing the 'your context name' context has changed since the database was created. Consider using Code First Migrations to update the database"
            

            //first check if it's an upgrade to existing plugin, then we won't be doing any insertions
            var mobSocialSettings =_settingService.LoadSetting<mobSocialSettings>();

            if (mobSocialSettings.Version != 0)
            {
                //an installation is already there, so let's skip the step
                if (!MobSocialConstant.SuiteInstallation) //if it's a suite installation, better not call the base method
                    base.Install();
                return;
            }
            //settings
            mobSocialSettings = new mobSocialSettings() {
                Version = MobSocialConstant.ReleaseVersion,
                ProfilePictureSize = 100,
                WidgetZone = "after_header_links",
                PeopleSearchAutoCompleteEnabled = true,
                PeopleSearchAutoCompleteNumberOfResults = 10,
                EventPageSearchAutoCompleteNumberOfResults = 10,
                PeopleSearchTermMinimumLength = 3,
                EventPageSearchTermMinimumLength = 1,
                ShowProfileImagesInSearchAutoComplete = true,
                CustomerAlbumPictureThumbnailWidth = 290,
                MaximumMainAlbumPictures = 10,
                MaximumMainAlbumVideos = 10,
                EventPageAttendanceThumbnailSize = 25,
                UninvitedFriendsNumberOfResults = 20,
                CustomerProfileUpdateViewCountAfterNumberOfSeconds = 180, // 3 minutes,
                FacebookWebsiteAppId = "1234567890123456",
                BusinessPageSearchUrl = "BusinessSearch",
                ArtistPageMainImageSize = 300,
                ArtistPageThumbnailSize = 150,
                // Would you like to replace the api key with your own? 
                // Find more info here by contacting us at info@skatemob.com
                EchonestAPIKey = "DQFW7ZCMHBBLMLVFE",
                // Would you like to replace the partner id with your own? 
                // Find more info here by contacting us at info@skatemob.com
                SevenDigitalPartnerId = "9378",
                SongFileMaximumUploadSize = _config.MaxRequestLength,
                SongFileSampleMaximumUploadSize = _config.MaxRequestLength,
                PurchasedSongFeePercentage = 30,
                ShowVideoThumbnailsForBattles = true,
                DefaultVotingChargeForPaidVoting = 0.99m,
                EnableFacebookInvite = true,
                MacroPaymentsFixedPaymentProcessingFee = 5,
                MicroPaymentsFixedPaymentProcessingFee = 0.05m,
                MacroPaymentsPaymentProcessingPercentage = 3.5m,
                MicroPaymentsPaymentProcessingPercentage = 5,
                VideoUploadReminderEmailThresholdDays = 5,
                BattleVoteReminderEmailThresholdDays = 5,
                TimelineSmallImageWidth = 300
            };

            //save distribution percentages as strings
            _settingService.SetSetting("winner_distribution_1", "100");
            _settingService.SetSetting("winner_distribution_2", "60+40");
            _settingService.SetSetting("winner_distribution_3", "45+35+20");
            _settingService.SetSetting("winner_distribution_4", "45+25+20+10");
            _settingService.SetSetting("winner_distribution_5", "40+25+20+10+5");

            var mediaSettings = new MediaSettings() {
                AvatarPictureSize = 200
            };



            AddLocaleResourceStrings();



            _settingService.SaveSetting(mediaSettings);
            _settingService.SaveSetting(mobSocialSettings);


            InsertMessageTemplates();

            AddScheduledTasks();



            _context.Install();
            if (!MobSocialConstant.SuiteInstallation)
                base.Install();//for suite installation, we don't have plugin descriptor, so avoid this call

        }






        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("MobSocial.MessageButtonText");
            this.DeletePluginLocaleResource("MobSocial.AddFriendButtonText");
            this.DeletePluginLocaleResource("MobSocial.FriendsLabelText");
            this.DeletePluginLocaleResource("MobSocial.FriendRequestSentLabel");
            this.DeletePluginLocaleResource("MobSocial.ConfirmFriendButtonText");
            this.DeletePluginLocaleResource("SearchDropdown.PeopleSearchText");

            //config page locales
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.ProfilePictureSize");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.ProfilePictureSize.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.WidgetZone");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.WidgetZone.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.ShowProfileImagesInSearchAutoComplete");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.ShowProfileImagesInSearchAutoComplete.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.PeopleSearchTermMinimumLength");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.PeopleSearchTermMinimumLength.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.PeopleSearchAutoCompleteNumberOfResults");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.PeopleSearchAutoCompleteNumberOfResults.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.PeopleSearchAutoCompleteEnabled");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.PeopleSearchAutoCompleteEnabled.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.CustomerAlbumPictureThumbnailWidth");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.CustomerAlbumPictureThumbnailWidth.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MaximumMainAlbumPictures");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MaximumMainAlbumPictures.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MaximumMainAlbumVideos");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MaximumMainAlbumVideos.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EventPageSearchTermMinimumLength");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EventPageSearchTermMinimumLength.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EventPageSearchAutoCompleteNumberOfResults");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EventPageSearchAutoCompleteNumberOfResults.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EventPageAttendanceThumbnailSize");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EventPageAttendanceThumbnailSize.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.UninvitedFriendsNumberOfResults");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.UninvitedFriendsNumberOfResults.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.CustomerProfileUpdateViewCountAfterNumberOfSeconds");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.CustomerProfileUpdateViewCountAfterNumberOfSeconds.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.FacebookWebsiteAppId");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.FacebookWebsiteAppId.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.BusinessPageSearchUrl");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.BusinessPageSearchUrl.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EchonestAPIKey");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EchonestAPIKey.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.ArtistPageMainImageSize");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.ArtistPageMainImageSize.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.ArtistPageThumbnailSize");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.ArtistPageThumbnailSize.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.SevenDigitalPartnerId");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.SevenDigitalPartnerId.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.SongFileMaximumUploadSize");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.SongFileMaximumUploadSize.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.SongFileSampleMaximumUploadSize");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.SongFileSampleMaximumUploadSize.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.PurchasedSongFeePercentage");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.PurchasedSongFeePercentage.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.ShowVideoThumbnailsForBattles");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.ShowVideoThumbnailsForBattles.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.DefaultVotingChargeForPaidVoting");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.DefaultVotingChargeForPaidVoting.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EnableAutomaticMigrations");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EnableAutomaticMigrations.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EnableFacebookInvite");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EnableFacebookInvite.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MicroMacroPaymentSwitchingAmount");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MicroMacroPaymentSwitchingAmount.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MicroPaymentsFixedPaymentProcessingFee");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MicroPaymentsFixedPaymentProcessingFee.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MicroPaymentsPaymentProcessingPercentage");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MicroPaymentsPaymentProcessingPercentage.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MacroPaymentsFixedPaymentProcessingFee");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MacroPaymentsFixedPaymentProcessingFee.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MacroPaymentsPaymentProcessingPercentage");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MacroPaymentsPaymentProcessingPercentage.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.SiteOwnerSponsorshipPercentage");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.SiteOwnerSponsorshipPercentage.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.BattleHostSponsorshipPercentage");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.BattleHostSponsorshipPercentage.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.VideoUploadReminderEmailThresholdDays");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.VideoUploadReminderEmailThresholdDays.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.BattleVoteReminderEmailThresholdDays");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.BattleVoteReminderEmailThresholdDays.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.TimelineSmallImageWidth");
            this.DeletePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.TimelineSmallImageWidth.Hint");

            // do not remove core locales

            RemoveScheduledTask("Nop.Plugin.WebApi.MobSocial.Tasks.FriendRequestNotificationTask");
            RemoveScheduledTask("Nop.Plugin.WebApi.MobSocial.Tasks.VideoBattlesStatusUpdateTask");

            //delete message templates
            DeleteMessageTemplates();

            //settings
            _settingService.DeleteSetting<mobSocialSettings>();

            _context.Uninstall();

            if (!MobSocialConstant.SuiteInstallation)
                base.Uninstall();
        }

        #endregion

        #region Tasks
        public void SendFriendRequestNotifications()
        {
            _mobSocialService.SendFriendRequestNotifications();
        }

        public void SendProductReviewNotifications()
        {
            _mobSocialService.SendProductReviewNotifications();
        }


        public void SetScheduledVideoBattlesOpenOrClosed()
        {
            _videoBattleService.SetScheduledBattlesOpenOrClosed();
        }


        #endregion

        public bool Authenticate()
        {
            return true;
        }

        #region Helper Methods
        private void AddScheduledTasks()
        {
            const int every24hrs = 24 * 60 * 60;
            AddScheduledTask("Friend Request Notification Task", every24hrs, false, false, "Nop.Plugin.WebApi.MobSocial.Tasks.FriendRequestNotificationTask, Nop.Plugin.WebApi.MobSocial");
            AddScheduledTask("Product Review Notification Task", every24hrs, false, false, "Nop.Plugin.WebApi.MobSocial.Tasks.ProductReviewNotificationTask, Nop.Plugin.WebApi.MobSocial");

            const int every5Min = 5 * 60;
            AddScheduledTask("Video Battle Status Update Task", every5Min, true, false, "Nop.Plugin.WebApi.MobSocial.Tasks.VideoBattlesStatusUpdateTask, Nop.Plugin.WebApi.MobSocial");

            const int every8Hrs = 8 * 60 * 60;
            AddScheduledTask("Reminder Notifications Task", every8Hrs, true, false, "Nop.Plugin.WebApi.MobSocial.Tasks.ReminderNotificationsTask, Nop.Plugin.WebApi.MobSocial");


        }


        internal void AddScheduledTask(string name, int seconds, bool enabled, bool stopOnError, string type)
        {
            var task = _scheduleTaskService.GetTaskByType(type);

            if (task != null)
                return;
            task = new ScheduleTask {
                Name = name,
                Seconds = seconds,
                Type = type,
                Enabled = enabled,
                StopOnError = stopOnError,
            };

            _scheduleTaskService.InsertTask(task);
        }


        private void RemoveScheduledTask(string type)
        {
            var task = _scheduleTaskService.GetTaskByType(type);

            if (task != null)
                _scheduleTaskService.DeleteTask(task);

        }



        private void AddLocaleResourceStrings()
        {
            this.AddOrUpdatePluginLocaleResource("MobSocial.MessageButtonText", "Send Message");
            this.AddOrUpdatePluginLocaleResource("MobSocial.AddFriendButtonText", "Add Friend");
            this.AddOrUpdatePluginLocaleResource("MobSocial.FriendsLabelText", "Friends");
            this.AddOrUpdatePluginLocaleResource("MobSocial.FriendRequestSentLabel", "Friend Request Sent!");
            this.AddOrUpdatePluginLocaleResource("MobSocial.ConfirmFriendButtonText", "Confirm");
            this.AddOrUpdatePluginLocaleResource("MobSocial.ConfirmedButtonText", "Confirmed!");
            this.AddOrUpdatePluginLocaleResource("SearchDropdown.PeopleSearchText", "People");
            this.AddOrUpdatePluginLocaleResource("SearchDropdown.EventPageSearchText", "Events");
            this.AddOrUpdatePluginLocaleResource("SearchDropdown.BusinessPageSearchText", "Businesses");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.MobSocial.AdminMenu.Text", "Social Network");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.MobSocial.AdminMenu.SubMenu.ManageTeamPage", "Manage Team Pages");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.MobSocial.AdminMenu.SubMenu.ManageEventPage", "Manage Event Pages");
            this.AddOrUpdatePluginLocaleResource("Admin.EventPage.BackToList", "Back to list");
            this.AddOrUpdatePluginLocaleResource("BusinessPages.HoursOfOperationText", "Hours of Operation");
            this.AddOrUpdatePluginLocaleResource("BusinessPages.HeaderMenuName", "Businesses");
            this.AddOrUpdatePluginLocaleResource("ArtistPages.SendPaymentMessageText", "When my song purchases reach $10 net amount, send payment to:");


            // Update core locales. do not remove core locales during uninstall
            this.AddOrUpdatePluginLocaleResource("Profile.ProfileOf", "{0}");
            this.AddOrUpdatePluginLocaleResource("Account.Avatar", "Profile Picture");
            this.AddOrUpdatePluginLocaleResource("Account.Avatar.MaximumUploadedFileSize", "Maximum profile picture size is {0} bytes");
            this.AddOrUpdatePluginLocaleResource("Account.Avatar.RemoveAvatar", "Remove Profile Picture");
            this.AddOrUpdatePluginLocaleResource("Account.Avatar.UploadRules", "Profile Picture must be in GIF or JPEG format with the maximum size of 20 KB");

            //locales for config page
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.ProfilePictureSize", "Profile Picture Size");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.ProfilePictureSize.Hint", "Default size of profile picture");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.WidgetZone", "Widget Zone");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.WidgetZone.Hint", "The widget zone name where widget is displayed");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.ShowProfileImagesInSearchAutoComplete", "Include images in autocomplete results");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.ShowProfileImagesInSearchAutoComplete.Hint", "Include images in autocomplete results");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.PeopleSearchTermMinimumLength", "People search minimum length");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.PeopleSearchTermMinimumLength.Hint", "People search minimum length");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.PeopleSearchAutoCompleteNumberOfResults", "Autocomplete number of results");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.PeopleSearchAutoCompleteNumberOfResults.Hint", "");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.PeopleSearchAutoCompleteEnabled", "Enable auto complete");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.PeopleSearchAutoCompleteEnabled.Hint", "Check to enable autocomplete");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.CustomerAlbumPictureThumbnailWidth", "Album Thumbnail Width");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.CustomerAlbumPictureThumbnailWidth.Hint", "Album Thumbnail Width");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MaximumMainAlbumPictures", "Maximum main album pictures");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MaximumMainAlbumPictures.Hint", "Maximum main album pictures");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MaximumMainAlbumVideos", "Maximum main album videos");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MaximumMainAlbumVideos.Hint", "Maximum main album videos");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EventPageSearchTermMinimumLength", "Event search minimum length");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EventPageSearchTermMinimumLength.Hint", "Event search minimum length");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EventPageSearchAutoCompleteNumberOfResults", "Event search autocomplete results");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EventPageSearchAutoCompleteNumberOfResults.Hint", "Event search autocomplete results");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EventPageAttendanceThumbnailSize", "Event page attendance thumbnail size");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EventPageAttendanceThumbnailSize.Hint", "Event page attendance thumbnail size");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.UninvitedFriendsNumberOfResults", "Uninvited friends number of results");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.UninvitedFriendsNumberOfResults.Hint", "Uninvited friends number of results");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.CustomerProfileUpdateViewCountAfterNumberOfSeconds", "View count update wait time");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.CustomerProfileUpdateViewCountAfterNumberOfSeconds.Hint", "The number of seconds to wait before view count for a profile is updated");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.FacebookWebsiteAppId", "Facebook App Id");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.FacebookWebsiteAppId.Hint", "Facebook App Id");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.BusinessPageSearchUrl", "Business page search url");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.BusinessPageSearchUrl.Hint", "Business page search url");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EchonestAPIKey", "Echonest api key");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EchonestAPIKey.Hint", "Echonest api key");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.ArtistPageMainImageSize", "Artist Page main image size");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.ArtistPageMainImageSize.Hint", "Artist page main image size");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.ArtistPageThumbnailSize", "Artist page thumbnail image size");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.ArtistPageThumbnailSize.Hint", "Artist page thumbnail image size");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.SevenDigitalPartnerId", "7Digital partner id");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.SevenDigitalPartnerId.Hint", "7Digital partner id");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.SongFileMaximumUploadSize", "Song file maximum upload size");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.SongFileMaximumUploadSize.Hint", "Song file maximum upload size");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.SongFileSampleMaximumUploadSize", "Song file sample maximum upload size");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.SongFileSampleMaximumUploadSize.Hint", "Song file sample maximum upload size");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.PurchasedSongFeePercentage", "Song fee percentage");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.PurchasedSongFeePercentage.Hint", "Song fee percentage");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.ShowVideoThumbnailsForBattles", "Show video thumbnails for video battles");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.ShowVideoThumbnailsForBattles.Hint", "Show video thumbnails for video battles");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.DefaultVotingChargeForPaidVoting", "Default voting charge for paid voting");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.DefaultVotingChargeForPaidVoting.Hint", "Default voting charge for paid voting");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EnableAutomaticMigrations", "Enable automatic migrations");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EnableAutomaticMigrations.Hint", "Enable automatic migrations");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EnableFacebookInvite", "Enable Facebook invite");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.EnableFacebookInvite.Hint", "Enable Facebook invite");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MicroMacroPaymentSwitchingAmount", "Micro to Macro payment switching amount");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MicroMacroPaymentSwitchingAmount.Hint", "Micro to Macro payment switching amount");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MicroPaymentsFixedPaymentProcessingFee", "Micro payments fixed processing fee");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MicroPaymentsFixedPaymentProcessingFee.Hint", "Micro payments fixed processing fee");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MicroPaymentsPaymentProcessingPercentage", "Micro payments processing fee percentage");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MicroPaymentsPaymentProcessingPercentage.Hint", "Micro payments processing fee percentage");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MacroPaymentsFixedPaymentProcessingFee", "Macro payments fixed processing fee");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MacroPaymentsFixedPaymentProcessingFee.Hint", "Macro payments fixed processing fee");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MacroPaymentsPaymentProcessingPercentage", "Macro payments processing fee percentage");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.MacroPaymentsPaymentProcessingPercentage.Hint", "Macro payments processing fee percentage");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.SiteOwnerSponsorshipPercentage", "Sponsorship percentage");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.SiteOwnerSponsorshipPercentage.Hint", "This is the percentage of amount that your website gets from total sponsorships");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.BattleHostSponsorshipPercentage", "Sponsorship percentage for battle host");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.BattleHostSponsorshipPercentage.Hint", "This is the percentage of amount that battle host gets from total sponsorships");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.VideoUploadReminderEmailThresholdDays", "Video upload reminder threshold");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.VideoUploadReminderEmailThresholdDays.Hint", "Number of days after which reminder for video uploads should be sent");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.BattleVoteReminderEmailThresholdDays", "Battle vote reminder threshold");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.BattleVoteReminderEmailThresholdDays.Hint", "Number of days after which reminder for voting should be sent");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.TimelineSmallImageWidth", "Timeline small image width");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.WebApi.MobSocial.TimelineSmallImageWidth.Hint", "Timeline small image width");
        }

        private void InsertMessageTemplates()
        {
           
            // Require user to login in order to view who and confirm the requests. Curiousity will drive traffic back to the site. - Bruce Leggett
            var friendRequestNotification = new MessageTemplate() {
                Name = "MobSocial.FriendRequestNotification",
                Subject = "You have a new friend request at %Store.Name%",
                Body = "You have a new friend request!<br/><br/>" +
                       "<a href=\"%Store.URL%\">Log in</a> to view and confirm your friend request.",
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId,
                IsActive = true,
                LimitedToStores = false

            };
            _messageTemplateService.InsertMessageTemplate(friendRequestNotification);


            // Send periodic friend request reminders, but not too many that frustrate users - Bruce Leggett
            var friendRequestReminderNotification = new MessageTemplate() {
                Name = "MobSocial.PendingFriendRequestNotification",
                Subject = "You have pending friend requests at %Store.Name%",
                Body = "You have friends waiting for you to confirm their requests!<br/><br/>" +
                       "<a href=\"%Store.URL%\">Log in</a> to view and confirm your friend requests.",
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId,
                IsActive = true,
                LimitedToStores = false

            };
            _messageTemplateService.InsertMessageTemplate(friendRequestReminderNotification);

            // Require user to login in order to view what event - Bruce Leggett
            var eventInvitationNotification = new MessageTemplate() {
                Name = "MobSocial.EventInvitationNotification",
                Subject = "You have been invited to an event on %Store.Name%",
                Body = "You have just been invited to an event!<br/><br/>" +
                       "<a href=\"%Store.URL%\">Log in</a> to view the event invitation.",
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId,
                IsActive = true,
                LimitedToStores = false

            };
            _messageTemplateService.InsertMessageTemplate(eventInvitationNotification);


            var productReviewNotification = new MessageTemplate() {
                Name = "MobSocial.ProductReviewNotification",
                Subject = "How do you like the products you ordered?",
                Body = "Hi %Customer.FirstName%,<br/><br/> What do you think about the products you've ordered? What do you think about the products you've ordered? Click on the products below to write a review and let us and others know what you think?<br/><br/>" +
                       "%ProductUrls% <br/><br/>" +
                       "Thanks!<br/> %Store.Name% Team",
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId,
                IsActive = true,
                LimitedToStores = false

            };
            _messageTemplateService.InsertMessageTemplate(productReviewNotification);



            var someoneSentYouASongNotification = new MessageTemplate() {
                Name = "MobSocial.SomeoneSentYouASongNotification",
                Subject = "%Friend.FirstName% sent you a song!",
                Body = "<a href=\"%Store.URL%\">Log in</a> to %Friend.FirstName%'s song to you.",
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(someoneSentYouASongNotification);


            var someoneChallengedForBattleNotification = new MessageTemplate() {
                Name = "MobSocial.SomeoneChallengedYouForBattleNotification",
                Subject = "%Challenger.FirstName% challenged you for a video battle!",
                Body = "<a href=\"%VideoBattle.Url%\">Visit battle page</a> to accept the challege.",
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(someoneChallengedForBattleNotification);

            var videoBattleCompleteNotificationToParticipants = new MessageTemplate() {
                Name = "MobSocial.VideoBattleCompleteNotificationToParticipants",
                Subject = "%VideoBattle.Title% is complete!",
                Body = "<a href=\"%VideoBattle.Url%\">Visit Battle Page</a> to see the winner.",
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId,
                IsActive = true,
                LimitedToStores = false
            };
            _messageTemplateService.InsertMessageTemplate(videoBattleCompleteNotificationToParticipants);

            var videoBattleCompleteNotificationToVoters = new MessageTemplate() {
                Name = "MobSocial.VideoBattleCompleteNotificationToVoters",
                Subject = "%VideoBattle.Title% is complete!",
                Body = "<a href=\"%VideoBattle.Url%\">Visit Battle Page</a> to see the winner.",
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId,
                IsActive = true,
                LimitedToStores = false
            };
            _messageTemplateService.InsertMessageTemplate(videoBattleCompleteNotificationToVoters);

            var someoneInvitedYouToVote = new MessageTemplate() {
                Name = "MobSocial.SomeoneInvitedYouToVoteNotification",
                Subject = "You have been invited to judge %VideoBattle.Title%!",
                Body = "<a href=\"%VideoBattle.Url%\">Visit Battle Page</a> to judge the participants.",
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId,
                IsActive = true,
                LimitedToStores = false
            };
            _messageTemplateService.InsertMessageTemplate(someoneInvitedYouToVote);

            var voteReminderNotification = new MessageTemplate() {
                Name = "MobSocial.VoteReminderNotification",
                Subject = "You have been invited to judge %VideoBattle.Title%!",
                Body = "<a href=\"%VideoBattle.Url%\">Visit Battle Page</a> to judge the participants.",
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(voteReminderNotification);

            var battleSignupNotification = new MessageTemplate() {
                Name = "MobSocial.VideoBattleSignupNotification",
                Subject = "%Challenger.Name% has signed up for %VideoBattle.Title%!",
                Body = "<a href=\"%VideoBattle.Url%\">Visit Battle Page</a> to approve the participants.",
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(battleSignupNotification);

            var battleJoinNotification = new MessageTemplate() {
                Name = "MobSocial.VideoBattleJoinNotification",
                Subject = "%Challenger.Name% is also participating in %VideoBattle.Title%!",
                Body = "<a href=\"%VideoBattle.Url%\">Visit Battle Page</a> to view the participants.",
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(battleJoinNotification);

            var signupAcceptedNotification = new MessageTemplate() {
                Name = "MobSocial.VideoBattleSignupAcceptedNotification",
                Subject = "You have been approved to participate in %VideoBattle.Title%!",
                Body = "<a href=\"%VideoBattle.Url%\">Visit Battle Page</a> to view the battle.",
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(signupAcceptedNotification);


            var sponsorAppliedNotification = new MessageTemplate() {
                Name = "MobSocial.SponsorAppliedNotification",
                Subject = "%Sponsor.Name% wants to sponsor %Battle.Title%!",
                Body = "Visit <a href=\"%SponsorDashboard.Url%\">Sponsor Dashboard</a> to accept the sponsorship.",
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(sponsorAppliedNotification);

            var sponsorshipStatusChangeNotification = new MessageTemplate() {
                Name = "MobSocial.SponsorshipStatusChangeNotification",
                Subject = "Sponsorship for %Battle.Title% has been %Sponsorship.Status%",
                Body = "Visit <a href=\"%SponsorDashboard.Url%\">Sponsor Dashboard</a> to view the details.",
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(sponsorshipStatusChangeNotification);

            var xDaysToBattleStartNotification = new MessageTemplate()
            {
                Name = "MobSocial.xDaysToBattleStartNotification",
                Subject = "%Battle.Title% starts %Battle.StartDaysString%",
                Body = "Visit <a href=\"%VideoBattle.Url%\">Battle Page</a> to upload your video",
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(xDaysToBattleStartNotification);

            var xDaysToBattleEndNotification = new MessageTemplate() {
                Name = "MobSocial.xDaysToBattleEndNotification",
                Subject = "%Battle.Title% ends %Battle.EndDaysString%",
                Body = "Visit <a href=\"%VideoBattle.Url%\">Battle Page</a> to cast your vote",
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(xDaysToBattleEndNotification);

            var videoBattleDisqualifiedNotification = new MessageTemplate()
            {
                Name = "MobSocial.VideoBattleDisqualifiedNotification",
                Subject = "You have been disqualified from participating in %VideoBattle.Title%!",
                Body = "<a href=\"%VideoBattle.Url%\">Visit Battle Page</a> to view the reason.",
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(videoBattleDisqualifiedNotification);

            var videoBattleOpenNotification = new MessageTemplate() {
                Name = "MobSocial.VideoBattleOpenNotification",
                Subject = "The battle '%VideoBattle.Title%' is open now!",
                Body = "<a href=\"%VideoBattle.Url%\">Visit Battle Page</a> to see the battle.",
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(videoBattleOpenNotification);

            var someInvitedToJoinNotification = new MessageTemplate() {
                Name = "MobSocial.InvitationToJoinNotification",
                Subject = "%Customer.FirstName% has invited you to join " + _storeContext.CurrentStore.Name,
                Body = "Hi, " + "%Customer.FirstName% has invited you to join " + _storeContext.CurrentStore.Name + " <a href='%Invitation.Url%'>Click here</a> to join.",
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(someInvitedToJoinNotification);

        }

        private void DeleteMessageTemplates()
        {
            var messageTemplates = new List<string>()
            {
                "MobSocial.FriendRequestNotification",
                "MobSocial.PendingFriendRequestNotification",
                "MobSocial.EventInvitationNotification",
                "MobSocial.ProductReviewNotification",
                "MobSocial.SomeoneSentYouASongNotification",
                "MobSocial.SomeoneChallengedYouForBattleNotification",
                "MobSocial.VideoBattleCompleteNotificationToParticipants",
                "MobSocial.VideoBattleCompleteNotificationToVoters",
                "MobSocial.SomeoneInvitedYouToVoteNotification",
                "MobSocial.VoteReminderNotification",
                "MobSocial.VideoBattleSignupNotification",
                "MobSocial.VideoBattleJoinNotification",
                "MobSocial.VideoBattleSignupAcceptedNotification",
                "MobSocial.SponsorAppliedNotification",
                "MobSocial.SponsorshipStatusChangeNotification",
                "MobSocial.xDaysToBattleStartNotification",
                "MobSocial.xDaysToBattleEndNotification"
            };

            foreach (var template in messageTemplates)
            {
                var messageTemplate = _messageTemplateService.GetMessageTemplateByName(template, _storeContext.CurrentStore.Id);
                if (messageTemplate == null)
                    continue;

                _messageTemplateService.DeleteMessageTemplate(messageTemplate);
            }
        }
        #endregion


        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode() {
                Title = _localizationService.GetResource("Plugins.Widgets.MobSocial.AdminMenu.Text"),
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", null } },
                SystemName = "mobSocialRootMenu",
                Url = ""
            };

            var configureSubMenu = new SiteMapNode() {
                Title = "mobSocial Settings",
                ControllerName = "Configuration",
                ActionName = "Configure",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", null } },
            };

            menuItem.ChildNodes.Add(configureSubMenu);

            var skillSubMenu = new SiteMapNode() {
                Title = "Skills",
                ControllerName = "Configuration",
                ActionName = "ConfigureSkills",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", null } },
            };

            menuItem.ChildNodes.Add(skillSubMenu);

            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");

            if (pluginNode != null)
                pluginNode.ChildNodes.Add(menuItem);
            else
                rootNode.ChildNodes.Add(menuItem);
        }
    }
}

