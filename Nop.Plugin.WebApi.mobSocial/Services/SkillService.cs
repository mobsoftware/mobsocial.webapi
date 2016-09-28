using System.Collections.Generic;
using System.Linq;
using Mob.Core.Data;
using Mob.Core.Services;
using Nop.Core;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Services.Seo;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public class SkillService : BaseEntityService<Skill>, ISkillService
    {
        public SkillService(IMobRepository<Skill> repository) : base(repository)
        {
        }

        public SkillService(IMobRepository<Skill> repository, IWorkContext workContext, IUrlRecordService urlRecordService) : base(repository, workContext, urlRecordService)
        {
        }

        public override List<Skill> GetAll(string Term, int Count = 15, int Page = 1)
        {
            return
                Repository.Table.Where(x => x.SkillName.Contains(Term))
                    .OrderBy(x => x.DisplayOrder)
                    .Skip((Page - 1)*Count)
                    .Take(Count)
                    .ToList();
        }

        public IList<Skill> GetUserSkills(int userId)
        {
            return Repository.Table.Where(x => x.CustomerId == userId).ToList();
        }
    }
}