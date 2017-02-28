using Mob.Core.Data;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Data
{
    public class UserSkillMap : BaseMobEntityTypeConfiguration<UserSkill>
    {
        public UserSkillMap()
        {
            Property(x => x.DisplayOrder);
            Property(x => x.Description);
            Property(x => x.ExternalUrl);
            Property(x => x.SkillId);
            Property(x => x.UserId);
            Property(x => x.DateCreated);
            Property(x => x.DateUpdated);
        }
    }
}