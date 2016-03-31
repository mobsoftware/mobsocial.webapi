using Mob.Core;
using Mob.Core.Data;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Data
{
    public class CustomerLikeMap : BaseMobEntityTypeConfiguration<CustomerLike>
    {
        public CustomerLikeMap()
        {
            Property(x => x.CustomerId);
            Property(x => x.EntityId);
            Property(x => x.EntityName);
        } 
    }
}