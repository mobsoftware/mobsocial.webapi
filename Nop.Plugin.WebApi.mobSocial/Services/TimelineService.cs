using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Mob.Core.Data;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public class TimelineService : BaseEntityService<TimelinePost>, ITimelineService
    {
        public TimelineService(IMobRepository<TimelinePost> repository) : base(repository)
        {

        }

        public override List<TimelinePost> GetAll(string term, int count = 15, int page = 1)
        {
            var posts = Repository.Table;
            if (!string.IsNullOrEmpty(term))
                posts = posts.Where(x => x.Message.Contains(term));

            posts = posts.OrderByDescending(x => x.DateCreated);

            return posts.Skip(count*(page - 1)).Take(count).ToList();

        }

        public IList<TimelinePost> GetByEntityIds(string owerEntityType, int[] ownerEntityIds, int count = 1, int page = 15)
        {
            return Repository.Table.Where(x => x.OwnerEntityType == owerEntityType &&
                                               ownerEntityIds.Contains(x.OwnerId))
                .OrderByDescending(x => x.DateCreated)
                .Skip(count*(page - 1))
                .Take(count).ToList();
        }
    }
}
