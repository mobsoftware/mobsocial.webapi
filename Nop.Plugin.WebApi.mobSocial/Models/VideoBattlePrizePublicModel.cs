using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class VideoBattlePrizePublicModel : BaseNopModel
    {
        public string WinningPosition { get; set; }

        public string PrizeType { get; set; }

        public string FormattedPrize { get; set; }

        public string SponsorName { get; set; }

        public string SponsorCustomerUrl { get; set; }
    }
}