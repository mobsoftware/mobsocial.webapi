using Mob.Core;
using Mob.Core.Domain;
using Nop.Core.Domain.Seo;

namespace Nop.Plugin.WebApi.MobSocial.Domain
{
    public class CustomerFollow : BaseMobEntity
    {
        public int CustomerId { get; set; }

        public int FollowingEntityId { get; set; }

        public string FollowingEntityName { get; set; }
    }
}