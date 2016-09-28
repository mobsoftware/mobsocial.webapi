using Mob.Core.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Domain
{
    public class Skill : BaseMobEntity
    {
        public string SkillName { get; set; }

        public string Description { get; set; }

        public int CustomerId { get; set; }

        public int DisplayOrder { get; set; }
    }
}
