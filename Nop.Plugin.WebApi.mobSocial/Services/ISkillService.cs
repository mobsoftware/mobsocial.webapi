using System.Collections;
using System.Collections.Generic;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface ISkillService : IBaseEntityService<Skill>
    {
        IList<Skill> GetUserSkills(int userId);

        IList<Skill> GetSystemSkills(out int total, string search = "", int page = 1, int count = 15);
    }
}