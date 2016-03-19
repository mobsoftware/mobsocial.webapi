using System.Collections.Generic;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface ITimelineService : IBaseEntityService<TimelinePost>
    {
        IList<TimelinePost> GetByEntityIds(string owerEntityType, int[] ownerEntityIds, bool onlyPublishable = true, int count = 15, int page = 1);


    }
}