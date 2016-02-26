using Nop.Plugin.WebApi.MobSocial.Enums;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class SponsorsRequestModel : BaseNopModel
    {
        public int BattleId { get; set; }

        public BattleType BattleType { get; set; }

        public SponsorshipStatus SponsorshipStatus { get; set; }
    }
}