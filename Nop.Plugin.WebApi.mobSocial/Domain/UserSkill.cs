using Mob.Core.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Domain
{
    public class UserSkill : BaseMobEntity
    {
        public int SkillId { get; set; }

        public virtual Skill Skill { get; set; }

        public int UserId { get; set; }

        public string ExternalUrl { get; set; }

        public int DisplayOrder { get; set; }

        public string Description { get; set; }

    }
}