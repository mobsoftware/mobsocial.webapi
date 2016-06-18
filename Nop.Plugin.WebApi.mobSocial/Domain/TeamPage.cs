using System;
using System.Collections.Generic;
using Mob.Core.Domain;
using Nop.Core;
using Nop.Core.Domain.Seo;

namespace Nop.Plugin.WebApi.MobSocial.Domain
{
    public class TeamPage : BaseMobEntity, ISlugSupported
    {

        public TeamPage()
        {
            GroupPages = new List<GroupPage>();
            
        }

        public virtual List<GroupPage> GroupPages { get; set; }

        public virtual DateTime CreatedOn { get; set; }

        public virtual int CreatedBy { get; set; }

        public virtual DateTime UpdatedOn { get; set; }

        public virtual int UpdatedBy { get; set; }

        public virtual string Description { get; set; }

        public virtual string TeamPictureUrl { get; set; }

        public virtual string Name { get; set; }


    }
}