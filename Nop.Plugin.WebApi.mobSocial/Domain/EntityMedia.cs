using Mob.Core.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Domain
{
    public class EntityMedia : BaseMobEntity
    {
        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public int MediaId { get; set; }
    }
}
