using System.Data.Entity.ModelConfiguration;
using Mob.Core.Data;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Data
{

    public class CustomerVideoAlbumMap : BaseMobEntityTypeConfiguration<CustomerVideoAlbum>
    {

        public CustomerVideoAlbumMap()
        {
            //Map the additional properties
            Property(m => m.CustomerId);
            Property(m => m.Name);
            Property(m => m.DisplayOrder);
            Property(m => m.IsMainVideoAlbum);
           
            
        }

    }
}
