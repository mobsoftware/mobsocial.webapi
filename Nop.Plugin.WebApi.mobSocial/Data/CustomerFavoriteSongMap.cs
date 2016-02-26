using System.Data.Entity.ModelConfiguration;
using Mob.Core.Data;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Data
{

    public class CustomerFavoriteSongMap : BaseMobEntityTypeConfiguration<CustomerFavoriteSong>
    {

        public CustomerFavoriteSongMap()
        {
            //Map the additional properties
            Property(m => m.TrackId);
            Property(m => m.Title);
            Property(m => m.PreviewUrl);
            Property(m => m.DisplayOrder);
            Property(m => m.IsDeleted);

        }

    }
}
