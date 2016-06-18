using System;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models.TeamPage
{
    public class TeamPagePublicModel : BaseNopModel
    {
        public virtual DateTime CreatedOn { get; set; }

        public virtual DateTime UpdatedOn { get; set; }

        public CustomerProfilePublicModel CreatedBy { get; set; }

        public CustomerProfilePublicModel UpdatedBy { get; set; }

        public virtual string Description { get; set; }

        public virtual string TeamPictureUrl { get; set; }

        public virtual string Name { get; set; }
    }
}
