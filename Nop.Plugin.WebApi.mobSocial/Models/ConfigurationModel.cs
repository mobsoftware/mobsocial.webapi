using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public ConfigurationModel()
        {
            AvailableZones = new List<SelectListItem>();
        }

       

        [NopResourceDisplayName("Admin.ContentManagement.Widgets.ChooseZone")]
        public string ZoneId { get; set; }

        public IList<SelectListItem> AvailableZones { get; set; }

        public string Product2Name { get; set; }

        public int DisplayOrder { get; set; }

        public int Id { get; set; }

        
        public int ProfilePictureSize { get; set; }
        public bool ProfilePictureSize_OverrideForStore { get; set; }

        public string WidgetZone { get; set; }
        public bool WidgetZone_OverrideForStore { get; set; }

        public bool ShowProfileImagesInSearchAutoComplete { get; set; }
        public bool ShowProfileImagesInSearchAutoComplete_OverrideForStore { get; set; }

        public int PeopleSearchTermMinimumLength { get; set; }
        public bool PeopleSearchTermMinimumLength_OverrideForStore { get; set; }

        public int PeopleSearchAutoCompleteNumberOfResults { get; set; }
        public bool PeopleSearchAutoCompleteNumberOfResults_OverrideForStore { get; set; }

        public bool PeopleSearchAutoCompleteEnabled { get; set; }
        public bool PeopleSearchAutoCompleteEnabled_OverrideForStore { get; set; }

        public int CustomerAlbumPictureThumbnailWidth { get; set; }
        public bool CustomerAlbumPictureThumbnailWidth_OverrideForStore { get; set; }

        public int MaximumMainAlbumPictures { get; set; }
        public bool MaximumMainAlbumPictures_OverrideForStore { get; set; }

        public int MaximumMainAlbumVideos { get; set; }
        public bool MaximumMainAlbumVideos_OverrideForStore { get; set; }

        public int EventPageSearchTermMinimumLength { get; set; }
        public bool EventPageSearchTermMinimumLength_OverrideForStore { get; set; }

        public int EventPageSearchAutoCompleteNumberOfResults { get; set; }
        public bool EventPageSearchAutoCompleteNumberOfResults_OverrideForStore { get; set; }

        public int EventPageAttendanceThumbnailSize { get; set; }
        public bool EventPageAttendanceThumbnailSize_OverrideForStore { get; set; }

        public int UninvitedFriendsNumberOfResults { get; set; }
        public bool UninvitedFriendsNumberOfResults_OverrideForStore { get; set; }

        public int CustomerProfileUpdateViewCountAfterNumberOfSeconds { get; set; }
        public bool CustomerProfileUpdateViewCountAfterNumberOfSeconds_OverrideForStore { get; set; }

        public string FacebookWebsiteAppId { get; set; }
        public bool FacebookWebsiteAppId_OverrideForStore { get; set; }

        public string BusinessPageSearchUrl { get; set; }
        public bool BusinessPageSearchUrl_OverrideForStore { get; set; }

        public string EchonestAPIKey { get; set; }
        public bool EchonestAPIKey_OverrideForStore { get; set; }

        public int ArtistPageMainImageSize { get; set; }
        public bool ArtistPageMainImageSize_OverrideForStore { get; set; }

        public int ArtistPageThumbnailSize { get; set; }
        public bool ArtistPageThumbnailSize_OverrideForStore { get; set; }

        public string SevenDigitalPartnerId { get; set; }
        public bool SevenDigitalPartnerId_OverrideForStore { get; set; }

        public int SongFileMaximumUploadSize { get; set; }
        public bool SongFileMaximumUploadSize_OverrideForStore { get; set; }

        public int SongFileSampleMaximumUploadSize { get; set; }
        public bool SongFileSampleMaximumUploadSize_OverrideForStore { get; set; }

        public int PurchasedSongFeePercentage { get; set; }
        public bool PurchasedSongFeePercentage_OverrideForStore { get; set; }

        public bool ShowVideoThumbnailsForBattles { get; set; }
        public bool ShowVideoThumbnailsForBattles_OverrideForStore { get; set; }

        public decimal DefaultVotingChargeForPaidVoting { get; set; }
        public bool DefaultVotingChargeForPaidVoting_OverrideForStore { get; set; }

        public bool EnableAutomaticMigrations { get; set; }
        public bool EnableAutomaticMigrations_OverrideForStore { get; set; }

        public bool EnableFacebookInvite { get; set; }
        public bool EnableFacebookInvite_OverrideForStore { get; set; }

        //TODO: move these setings to a separate payment processing plugin
        public decimal MicroMacroPaymentSwitchingAmount { get; set; }
        public bool MicroMacroPaymentSwitchingAmount_OverrideForStore { get; set; }

        public decimal MicroPaymentsFixedPaymentProcessingFee { get; set; }
        public bool MicroPaymentsFixedPaymentProcessingFee_OverrideForStore { get; set; }

        public decimal MicroPaymentsPaymentProcessingPercentage { get; set; }
        public bool MicroPaymentsPaymentProcessingPercentage_OverrideForStore { get; set; }

        public decimal MacroPaymentsFixedPaymentProcessingFee { get; set; }
        public bool MacroPaymentsFixedPaymentProcessingFee_OverrideForStore { get; set; }

        public decimal MacroPaymentsPaymentProcessingPercentage { get; set; }
        public bool MacroPaymentsPaymentProcessingPercentage_OverrideForStore { get; set; }

        public decimal SiteOwnerSponsorshipPercentage { get; set; }
        public bool SiteOwnerSponsorshipPercentage_OverrideForStore { get; set; }

        public decimal BattleHostSponsorshipPercentage { get; set; }
        public bool BattleHostSponsorshipPercentage_OverrideForStore { get; set; }

        public int VideoUploadReminderEmailThresholdDays { get; set; }
        public bool VideoUploadReminderEmailThresholdDays_OverrideForStore { get; set; }

        public int BattleVoteReminderEmailThresholdDays { get; set; }
        public bool BattleVoteReminderEmailThresholdDays_OverrideForStore { get; set; }

        public int TimelineSmallImageWidth { get; set; }
        public bool TimelineSmallImageWidth_OverrideForStore { get; set; }

        public string MessageText { get; set; }

        public int ActiveStoreScopeConfiguration { get; set; }

    }
}