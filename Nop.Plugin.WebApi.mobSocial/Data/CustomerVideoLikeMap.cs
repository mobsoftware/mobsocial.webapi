using System.Data.Entity.ModelConfiguration;
using Mob.Core.Data;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Data
{

    public class CustomerVideoLikeMap : BaseMobEntityTypeConfiguration<CustomerVideoLike>
    {

        public CustomerVideoLikeMap()
        {
            //Map the additional properties
            Property(m => m.CustomerId);
            Property(m => m.CustomerVideoId);


        }

    }
}
