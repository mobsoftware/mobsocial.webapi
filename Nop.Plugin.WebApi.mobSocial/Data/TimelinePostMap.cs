using Mob.Core.Data;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Data
{
    public class TimelinePostMap : BaseMobEntityTypeConfiguration<TimelinePost>
    {
        public TimelinePostMap()
        {
            Property(x => x.OwnerId);
            Property(x => x.OwnerEntityType);
            Property(x => x.IsSponsored);
            Property(x => x.PostTypeName);
            Property(x => x.Message);
            Property(x => x.LinkedToEntityId);
            Property(x => x.LinkedToEntityName);
            Property(x => x.IsHidden);
            Property(x => x.PublishDate);
        }
    }
}
