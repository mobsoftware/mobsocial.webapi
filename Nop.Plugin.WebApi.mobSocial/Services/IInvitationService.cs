using System.Collections.Generic;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface IInvitationService : IBaseEntityService<Invitation>
    {
        IList<Invitation> GetInvitationsByInviter(int inviterId);

        IList<Invitation> GetInvitationsByInvitee(string inviteeEmail);
    }
}