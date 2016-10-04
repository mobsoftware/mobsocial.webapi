using System;
using System.Collections.Generic;
using System.Linq;
using Mob.Core.Data;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public class InvitationService : BaseEntityService<Invitation>, IInvitationService
    {
        public InvitationService(IMobRepository<Invitation> repository) : base(repository)
        {
        }

        public override List<Invitation> GetAll(string Term, int Count = 15, int Page = 1)
        {
            throw new NotImplementedException();
        }

        public IList<Invitation> GetInvitationsByInviter(int inviterId)
        {
            return Repository.Table.Where(x => x.InviterUserId == inviterId).ToList();
        }

        public IList<Invitation> GetInvitationsByInvitee(string inviteeEmail)
        {
            return Repository.Table.Where(x => x.InviteeEmailAddress == inviteeEmail).ToList();
        }
    }
}
