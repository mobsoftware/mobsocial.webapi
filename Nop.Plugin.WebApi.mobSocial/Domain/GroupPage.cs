using System.Collections.Generic;
using Mob.Core.Domain;
using Nop.Core;

namespace Nop.Plugin.WebApi.MobSocial.Domain
{

    public class GroupPage : BaseMobEntity
    {

        public GroupPage()
        {
            Members = new List<GroupPageMember>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string PayPalDonateUrl { get; set; }

        public int TeamId { get; set; }

        public virtual List<GroupPageMember> Members { get; set; }

        public virtual TeamPage Team { get; set; }

        /// <summary>
        /// Display order of this group on the Team Page
        /// </summary>
        public int DisplayOrder { get; set; }

        public bool IsDefault { get; set; }

    }

}



