using Mob.Core.Data;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Data
{
    public class SkillMap : BaseMobEntityTypeConfiguration<Skill>
    {
        public SkillMap()
        {
            Property(x => x.UserId);
            Property(x => x.Name);
            Property(x => x.DisplayOrder);
            Property(x => x.DateCreated).HasColumnType("datetime2");
            Property(x => x.DateUpdated).HasColumnType("datetime2");
            Property(x => x.FeaturedImageId);

        }
    }
}