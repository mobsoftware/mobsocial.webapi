using System;
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
                    .Skip((Page - 1) * Count)
                    .Take(Count)
                    .ToList();
        }

        public IList<Skill> GetUserSkills(int userId)
        {
            return Repository.Table.Where(x => x.CustomerId == userId).ToList();
        }

        public IList<Skill> GetSystemSkills(out int total, string search = "", int page = 1, int count = 15)
        {
            var q = Repository.Table.Where(x => x.CustomerId == 0 && (search == "" || x.SkillName.StartsWith(search)));
            total = q.Count(); //total records
            return
                q.OrderBy(x => x.SkillName)
                    .Skip(count*(page - 1))
                    .Take(count)
                    .ToList();
        }

        public IList<Skill> SearchSkills(string searchText, int page = 1, int count = 15)
        {
            searchText = searchText.ToLower();
            return
               Repository.Table
                   .Where(x => x.SkillName.ToLower().StartsWith(searchText))
                   .OrderBy(x => x.DisplayOrder)
                   .AsEnumerable()
                   .Distinct(new SkillComparer())
                   .Skip((page - 1) * count)
                   .Take(count)
                   .ToList();
        }
    }

    public class SkillComparer : IEqualityComparer<Skill>
    {
        public bool Equals(Skill x, Skill y)
        {
            return x.SkillName == y.SkillName;
        }

        public int GetHashCode(Skill obj)
        {
            return obj.SkillName.GetHashCode();
        }
    }
}