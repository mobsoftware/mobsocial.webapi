using Mob.Core.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Domain
{
    public class TimelinePost : BaseMobEntity
    {
        public int OwnerId { get; set; }

        public string OwnerEntityType { get; set; }

        public string PostTypeName { get; set; }

        public bool IsSponsored { get; set; }

        public string Message { get; set; }

        public string AdditionalAttributeValue { get; set; }

        public int LinkedToEntityId { get; set; }

        public string LinkedToEntityName { get; set; }

        public bool IsHidden { get; set; }
    }
}
