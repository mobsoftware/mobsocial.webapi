using System.Collections.Generic;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    /// <summary>
    /// Product service
    /// </summary>
    public interface ITeamPageService : IBaseEntityService<TeamPage>
    {
        TeamPage GetTeamPageByGroup(int groupId);

        /// <summary>
        /// Safely deletes a team after deleting the groups and member associations
        /// </summary>
        void SafeDelete(TeamPage team);

        /// <summary>
        /// Gets team pages by the ower
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        IList<TeamPage> GetTeamPagesByOwner(int ownerId);
    }

}
