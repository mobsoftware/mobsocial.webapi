using Mob.Core.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Domain
{
    public class CustomerComment : BaseMobEntity
    {
        public int CustomerId { get; set; }

        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public string CommentText { get; set; }

        public string AdditionalData { get; set; }
    }
}
