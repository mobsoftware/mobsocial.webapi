using System.Collections.Generic;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface ISkillService : IBaseEntityService<Skill>
    {
        IList<UserSkill> GetUserSkills(int userId);

        IList<Skill> GetAllSkills(out int total, string search = "", int page = 1, int count = 15);

        IList<Skill> SearchSkills(string searchText, int page = 1, int count = 15);
    }
}