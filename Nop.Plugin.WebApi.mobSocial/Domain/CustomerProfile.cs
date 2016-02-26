using System;
using Mob.Core;
using Mob.Core.Domain;
using Nop.Core;

namespace Nop.Plugin.WebApi.MobSocial.Domain
{
    public class CustomerProfile : BaseMobEntity, IFollowSupported
    {
        public int CustomerId { get; set; }
        public string AboutMe { get; set; }
        public string Website { get; set; }

    }

}
