using System.Data.Entity.ModelConfiguration;
using Mob.Core.Data;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Data
{

    public class BusinessPagePictureMap : BaseMobEntityTypeConfiguration<BusinessPagePicture>
    {

        public BusinessPagePictureMap()
        {
            Property(m => m.PictureId);
            Property(m => m.DisplayOrder);
            Property(m => m.EntityId).HasColumnName("BusinessPageId"); //backward compatibility
            
            
        }

    }
}
