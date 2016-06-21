using System.Collections.Generic;
using System.Linq;
using Mob.Core.Data;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public class TeamPageGroupMemberService : BaseEntityService<GroupPageMember>, ITeamPageGroupMemberService
    {
        private readonly IMobRepository<GroupPage> _groupPageRepository;

        public TeamPageGroupMemberService(IMobRepository<GroupPageMember> repository, 
            IMobRepository<GroupPage> groupPageRepository) : base(repository)
        {
            this._groupPageRepository = groupPageRepository;
        }

        public override List<GroupPageMember> GetAll(string Term, int Count = 15, int Page = 1)
        {
            throw new System.NotImplementedException();
        }

        public IList<GroupPageMember> GetGroupPageMembersForTeam(int teamId)
        {
            //find all groups for this team
            var allGroupsId = _groupPageRepository.Table.Where(x => x.TeamId == teamId).Select(x => x.Id);
            return Repository.Table.Where(x => allGroupsId.Contains(x.GroupPageId)).OrderBy(x => x.DisplayOrder).ToList();
        }

        public IList<GroupPageMember> GetGroupPageMembers(int groupId)
        {
            return Repository.Table.Where(x => x.GroupPageId == groupId).OrderBy(x => x.DisplayOrder).ToList();
        }

        public void DeleteGroupPageMember(int groupId, int memberId)
        {
            var groupMember = Repository.Table.FirstOrDefault(x => x.GroupPageId == groupId && x.CustomerId == memberId);
            if (groupMember != null)
                Delete(groupMember);
        }
    }
}