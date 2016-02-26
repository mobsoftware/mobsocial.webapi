using Mob.Core.Domain;
using Nop.Plugin.WebApi.MobSocial.Enums;

namespace Nop.Plugin.WebApi.MobSocial.Domain
{
    public class WatchedVideo: BaseMobEntity
    {
        public int VideoId { get; set; }

        public int CustomerId { get; set; }

        public VideoType VideoType { get; set; }
    }
}