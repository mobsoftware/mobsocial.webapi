using System.Collections.Generic;
using System.Linq;
using Mob.Core.Data;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    /// <summary>
    /// Product service
    /// </summary>
    public class TeamPageService : BaseEntityService<TeamPage>, ITeamPageService
    {

        private readonly IMobRepository<GroupPage> _groupPageRepository;
        private readonly IMobRepository<GroupPageMember> _groupPageMembeRepository;

        public TeamPageService(IMobRepository<TeamPage> teamPageRepository,
            IMobRepository<GroupPage> groupPageRepository, IMobRepository<GroupPageMember> groupPageMembeRepository) : base(teamPageRepository)
        {
            _groupPageRepository = groupPageRepository;
            _groupPageMembeRepository = groupPageMembeRepository;
        }


        public override List<TeamPage> GetAll(string Term, int Count = 15, int Page = 1)
        {
            var termLowerCase = Term.ToLower();
            return base.Repository.Table
                .Where(x => x.Name.ToLower().Contains(termLowerCase))
                .Skip((Page - 1) * Count)
                .Take(Count)
                .ToList();
        }

        public TeamPage GetTeamPageByGroup(int groupId)
        {
            //first let's query the team id of the group
            var group = _groupPageRepository.Table.FirstOrDefault(x => x.Id == groupId);
            //query the team page
            return @group == null ? null : GetById(@group.TeamId);

        }

        public void SafeDelete(TeamPage team)
        {
            var groupPageIds = team.GroupPages.Select(x => x.Id);
            //get group member associations
            var groupMembers = _groupPageMembeRepository.Table.Where(x => groupPageIds.Contains(x.GroupPageId)).ToList();

            //delete all group members

            while (groupMembers.Any())
                _groupPageMembeRepository.Delete(groupMembers.First());

            while (team.GroupPages.Any())
                //delete all groups
                _groupPageRepository.Delete(team.GroupPages.First());

            //delete the team
            Delete(team);

        }

        public IList<TeamPage> GetTeamPagesByOwner(int ownerId)
        {
            return Repository.Table.Where(x => x.CreatedBy == ownerId).ToList();
        }
    }

}



