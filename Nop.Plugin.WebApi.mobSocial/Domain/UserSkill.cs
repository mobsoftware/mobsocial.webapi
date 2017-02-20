using Mob.Core.Domain;
using Nop.Core.Domain.Customers;

namespace Nop.Plugin.WebApi.MobSocial.Domain
{
    public class UserSkill : BaseMobEntity
    {
        public int SkillId { get; set; }

        public virtual Skill Skill { get; set; }

        public int UserId { get; set; }

        public virtual Customer User { get; set; }

        public string ExternalUrl { get; set; }

        public int DisplayOrder { get; set; }

        public string Description { get; set; }

    }
}