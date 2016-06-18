using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models.TeamPage
{
    public class TeamPageGroupModel : BaseNopEntityModel
    {
        public int TeamId { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string PayPalDonateUrl { get; set; }

        public virtual int DisplayOrder { get; set; }

    }
}