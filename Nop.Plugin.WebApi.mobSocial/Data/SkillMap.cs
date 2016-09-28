using Mob.Core.Data;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Data
{
    public class SkillMap : BaseMobEntityTypeConfiguration<Skill>
    {
        public SkillMap()
        {
            Property(x => x.SkillName);
            Property(x => x.Description);
            Property(x => x.CustomerId);
            Property(x => x.DisplayOrder);
        }
    }
}
