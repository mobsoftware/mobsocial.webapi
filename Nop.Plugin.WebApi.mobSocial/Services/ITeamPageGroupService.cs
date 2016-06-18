using System.Collections.Generic;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface ITeamPageGroupService : IBaseEntityService<GroupPage>
    {
        IList<GroupPage> GetGroupPagesByTeamId(int teamId);

        /// <summary>
        /// Safely deletes a group page after deleting all the member associations
        /// </summary>
        /// <param name="groupPage"></param>
        void SafeDelete(GroupPage groupPage);
    }
}