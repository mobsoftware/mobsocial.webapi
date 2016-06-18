using System;
using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models.TeamPage
{
    public class TeamPageGroupPublicModel : BaseNopModel
    {
        public int Id { get; set; }

        public int TeamPageId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime UpdatedOn { get; set; }

        public DateTime UpdatedOnUtc { get; set; }

        public string PaypalDonateUrl { get; set; }

        public IList<CustomerProfilePublicModel> GroupMembers { get; set; }
    }
}