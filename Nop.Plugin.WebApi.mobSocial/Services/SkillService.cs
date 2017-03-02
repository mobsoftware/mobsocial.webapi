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
        private readonly IMobRepository<UserSkill> _userSkillDataRepository;

        public SkillService(IMobRepository<Skill> dataRepository, IMobRepository<UserSkill> userSkillDataRepository, IWorkContext workContext, IUrlRecordService urlRecordService) : base(dataRepository, workContext, urlRecordService)
        {
            _userSkillDataRepository = userSkillDataRepository;
        }

        public IList<UserSkill> GetUserSkills(int userId)
        {
            //get user's skill ids
            return _userSkillDataRepository.Table.Where(x => x.UserId == userId).ToList();
        }

        public IList<Skill> GetAllSkills(out int total, string search = "", int page = 1, int count = 15)
        {
            var q = Repository.Table.Where(x => search == "" || x.Name.StartsWith(search));
            total = q.Count(); //total records
            return
                q.OrderBy(x => x.Name)
                    .Skip(count*(page - 1))
                    .Take(count)
                    .ToList();
        }

        public IList<Skill> SearchSkills(string searchText, int page = 1, int count = 15)
        {
            searchText = searchText.ToLower();
            return
               Repository.Table.Where(x => x.Name.ToLower().StartsWith(searchText))
                   .OrderBy(x => x.DisplayOrder)
                   .AsEnumerable()
                   .Distinct(new SkillComparer())
                   .Skip((page - 1) * count)
                   .Take(count)
                   .ToList();
        }


        public override List<Skill> GetAll(string Term, int Count = 15, int Page = 1)
        {
            throw new System.NotImplementedException();
        }
    }

    public class SkillComparer : IEqualityComparer<Skill>
    {
        public bool Equals(Skill x, Skill y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(Skill obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}