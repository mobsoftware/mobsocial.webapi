using Mob.Core;
using Mob.Core.Domain;
using Nop.Core.Domain.Seo;

namespace Nop.Plugin.WebApi.MobSocial.Domain
{
    public class CustomerLike : BaseMobEntity
    {
        public int CustomerId { get; set; }

        public int EntityId { get; set; }

        public string EntityName { get; set; }
    }
}