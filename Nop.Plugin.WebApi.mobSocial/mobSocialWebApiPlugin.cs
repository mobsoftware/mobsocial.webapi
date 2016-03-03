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

        public MobSocialWebApiPlugin(MobSocialObjectContext context, mobSocialSettings mobSocialSettings,
            ISettingService settingService, IMessageTemplateService messageTemplateService,
            IScheduleTaskService scheduleTaskService,
            IMobSocialService mobSocialService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IVideoBattleService videoBattleService)
        {
            _context = context;
            _mobSocialSettings = mobSocialSettings;
            _settingService = settingService;
            _messageTemplateService = messageTemplateService;
            _scheduleTaskService = scheduleTaskService;
            _mobSocialService = mobSocialService;
            _localizationService = localizationService;
            _videoBattleService = videoBattleService;
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

            if (mobSocialSettings != null)
            {
                //an installation is already there, so let's skip the step
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
                BattleVoteReminderEmailThresholdDays = 5
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

            base.Install();

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
            // do not remove core locales

            RemoveScheduledTask("Nop.Plugin.WebApi.MobSocial.Tasks.FriendRequestNotificationTask, Nop.Plugin.WebApi.MobSocial");
            RemoveScheduledTask("Nop.Plugin.WebApi.MobSocial.Tasks.VideoBattlesStatusUpdateTask, Nop.Plugin.WebApi.MobSocial");

            //delete message templates
            DeleteMessageTemplates();

            //settings
            _settingService.DeleteSetting<mobSocialSettings>();

            _context.Uninstall();

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

        public Nop.Web.Framework.Menu.SiteMapNode BuildMenuItem()
        {


            return null;
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

        }

        private void InsertMessageTemplates()
        {
            // Require user to login in order to view who and confirm the requests. Curiousity will drive traffic back to the site. - Bruce Leggett
            var friendRequestNotification = new MessageTemplate() {
                Name = "MobSocial.FriendRequestNotification",
                Subject = "You have a new friend request at %Store.Name%",
                Body = "You have a new friend request!<br/><br/>" +
                       "<a href=\"%Store.URL%\">Log in</a> to view and confirm your friend request.",
                EmailAccountId = 1,
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
                EmailAccountId = 1,
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
                EmailAccountId = 1,
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
                EmailAccountId = 1,
                IsActive = true,
                LimitedToStores = false

            };
            _messageTemplateService.InsertMessageTemplate(productReviewNotification);



            var someoneSentYouASongNotification = new MessageTemplate() {
                Name = "MobSocial.SomeoneSentYouASongNotification",
                Subject = "%Friend.FirstName% sent you a song!",
                Body = "<a href=\"%Store.URL%\">Log in</a> to %Friend.FirstName%'s song to you.",
                EmailAccountId = 1,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(someoneSentYouASongNotification);


            var someoneChallengedForBattleNotification = new MessageTemplate() {
                Name = "MobSocial.SomeoneChallengedYouForBattleNotification",
                Subject = "%Challenger.FirstName% challenged you for a video battle!",
                Body = "<a href=\"%VideoBattle.Url%\">Visit battle page</a> to accept the challege.",
                EmailAccountId = 1,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(someoneChallengedForBattleNotification);

            var videoBattleCompleteNotificationToParticipants = new MessageTemplate() {
                Name = "MobSocial.VideoBattleCompleteNotificationToParticipants",
                Subject = "%VideoBattle.Title% is complete!",
                Body = "<a href=\"%VideoBattle.Url%\">Visit Battle Page</a> to see the winner.",
                EmailAccountId = 1,
                IsActive = true,
                LimitedToStores = false
            };
            _messageTemplateService.InsertMessageTemplate(videoBattleCompleteNotificationToParticipants);

            var videoBattleCompleteNotificationToVoters = new MessageTemplate() {
                Name = "MobSocial.VideoBattleCompleteNotificationToVoters",
                Subject = "%VideoBattle.Title% is complete!",
                Body = "<a href=\"%VideoBattle.Url%\">Visit Battle Page</a> to see the winner.",
                EmailAccountId = 1,
                IsActive = true,
                LimitedToStores = false
            };
            _messageTemplateService.InsertMessageTemplate(videoBattleCompleteNotificationToVoters);

            var someoneInvitedYouToVote = new MessageTemplate() {
                Name = "MobSocial.SomeoneInvitedYouToVoteNotification",
                Subject = "You have been invited to judge %VideoBattle.Title%!",
                Body = "<a href=\"%VideoBattle.Url%\">Visit Battle Page</a> to judge the participants.",
                EmailAccountId = 1,
                IsActive = true,
                LimitedToStores = false
            };
            _messageTemplateService.InsertMessageTemplate(someoneInvitedYouToVote);

            var voteReminderNotification = new MessageTemplate() {
                Name = "MobSocial.VoteReminderNotification",
                Subject = "You have been invited to judge %VideoBattle.Title%!",
                Body = "<a href=\"%VideoBattle.Url%\">Visit Battle Page</a> to judge the participants.",
                EmailAccountId = 1,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(voteReminderNotification);

            var battleSignupNotification = new MessageTemplate() {
                Name = "MobSocial.VideoBattleSignupNotification",
                Subject = "%Challenger.Name% has signed up for %VideoBattle.Title%!",
                Body = "<a href=\"%VideoBattle.Url%\">Visit Battle Page</a> to approve the participants.",
                EmailAccountId = 1,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(battleSignupNotification);

            var battleJoinNotification = new MessageTemplate() {
                Name = "MobSocial.VideoBattleJoinNotification",
                Subject = "%Challenger.Name% is also participating in %VideoBattle.Title%!",
                Body = "<a href=\"%VideoBattle.Url%\">Visit Battle Page</a> to view the participants.",
                EmailAccountId = 1,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(battleJoinNotification);

            var signupAcceptedNotification = new MessageTemplate() {
                Name = "MobSocial.VideoBattleSignupAcceptedNotification",
                Subject = "You have been approved to participate in %VideoBattle.Title%!",
                Body = "<a href=\"%VideoBattle.Url%\">Visit Battle Page</a> to view the battle.",
                EmailAccountId = 1,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(signupAcceptedNotification);


            var sponsorAppliedNotification = new MessageTemplate() {
                Name = "MobSocial.SponsorAppliedNotification",
                Subject = "%Sponsor.Name% wants to sponsor %Battle.Title%!",
                Body = "Visit <a href=\"%SponsorDashboard.Url%\">Sponsor Dashboard</a> to accept the sponsorship.",
                EmailAccountId = 1,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(sponsorAppliedNotification);

            var sponsorshipStatusChangeNotification = new MessageTemplate() {
                Name = "MobSocial.SponsorshipStatusChangeNotification",
                Subject = "Sponsorship for %Battle.Title% has been %Sponsorship.Status%",
                Body = "Visit <a href=\"%SponsorDashboard.Url%\">Sponsor Dashboard</a> to view the details.",
                EmailAccountId = 1,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(sponsorshipStatusChangeNotification);

            var xDaysToBattleStartNotification = new MessageTemplate()
            {
                Name = "MobSocial.xDaysToBattleStartNotification",
                Subject = "%Battle.Title% starts %Battle.StartDaysString%",
                Body = "Visit <a href=\"%VideoBattle.Url%\">Battle Page</a> to upload your video",
                EmailAccountId = 1,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(xDaysToBattleStartNotification);

            var xDaysToBattleEndNotification = new MessageTemplate() {
                Name = "MobSocial.xDaysToBattleEndNotification",
                Subject = "%Battle.Title% ends %Battle.EndDaysString%",
                Body = "Visit <a href=\"%VideoBattle.Url%\">Battle Page</a> to cast your vote",
                EmailAccountId = 1,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(xDaysToBattleEndNotification);

            var videoBattleDisqualifiedNotification = new MessageTemplate()
            {
                Name = "MobSocial.VideoBattleDisqualifiedNotification",
                Subject = "You have been disqualified from participating in %VideoBattle.Title%!",
                Body = "<a href=\"%VideoBattle.Url%\">Visit Battle Page</a> to view the reason.",
                EmailAccountId = 1,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(videoBattleDisqualifiedNotification);

            var videoBattleOpenNotification = new MessageTemplate() {
                Name = "MobSocial.VideoBattleOpenNotification",
                Subject = "The battle '%VideoBattle.Title%' is open now!",
                Body = "<a href=\"%VideoBattle.Url%\">Visit Battle Page</a> to see the battle.",
                EmailAccountId = 1,
                IsActive = true,
                LimitedToStores = false
            };

            _messageTemplateService.InsertMessageTemplate(videoBattleOpenNotification);

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

           

        }
    }
}

