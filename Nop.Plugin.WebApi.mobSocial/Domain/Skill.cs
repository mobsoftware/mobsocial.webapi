using Mob.Core;
using Mob.Core.Domain;
using Nop.Core.Domain.Seo;

namespace Nop.Plugin.WebApi.MobSocial.Domain
{
    public class Skill : BaseMobEntity, ISlugSupported, IFollowSupported
    {
        public string SkillName { get; set; }

        public int DisplayOrder { get; set; }

        public int UserId { get; set; }

        public int FeaturedImageId { get; set; }

        public string Name { get; set; }
    }
}
