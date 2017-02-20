using System.Collections.Generic;
using Mob.Core.Data;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public class UserSkillService : BaseEntityService<UserSkill>, IUserSkillService
    {
        public UserSkillService(IMobRepository<UserSkill> dataRepository) : base(dataRepository) {}

        public override List<UserSkill> GetAll(string Term, int Count = 15, int Page = 1)
        {
            throw new System.NotImplementedException();
        }
    }
}