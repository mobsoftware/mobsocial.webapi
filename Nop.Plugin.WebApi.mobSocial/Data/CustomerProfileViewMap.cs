using System.Data.Entity.ModelConfiguration;
using Mob.Core.Data;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Data
{

    public class CustomerProfileViewMap : BaseMobEntityTypeConfiguration<CustomerProfileView>
    {

        public CustomerProfileViewMap()
        {
            //Map the additional properties
            Property(m => m.CustomerId);
            Property(m => m.ViewerCustomerId);
            Property(m => m.Views);



        }

    }
}
