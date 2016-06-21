using System.Collections.Generic;
using System.Linq;
using Mob.Core.Data;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public class TeamPageGroupService : BaseEntityService<GroupPage>, ITeamPageGroupService
    {
        private readonly IMobRepository<GroupPageMember> _memberRepository;

        public TeamPageGroupService(IMobRepository<GroupPage> repository, IMobRepository<GroupPageMember> memberRepository) : base(repository)
        {
            this._memberRepository = memberRepository;
        }

        public override List<GroupPage> GetAll(string Term, int Count = 15, int Page = 1)
        {
            throw new System.NotImplementedException();
        }

        public IList<GroupPage> GetGroupPagesByTeamId(int teamId)
        {
            return Repository.Table.Where(x => x.TeamId == teamId).OrderBy(x => x.DisplayOrder).ToList();
        }

        public void SafeDelete(GroupPage groupPage)
        {
            while(groupPage.Members.Any())
                _memberRepository.Delete(groupPage.Members.First());

            Delete(groupPage);
        }
    }
}
