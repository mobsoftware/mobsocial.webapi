using System.Data.Entity.ModelConfiguration;
using Mob.Core.Data;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Data
{

    public class EventPagePictureMap : BaseMobEntityTypeConfiguration<EventPagePicture>
    {

        public EventPagePictureMap()
        {
            Property(m => m.PictureId);
            Property(m => m.DisplayOrder);
            Property(m => m.EntityId).HasColumnName("EventPageId"); //backward compatibility
            
        }

    }
}
