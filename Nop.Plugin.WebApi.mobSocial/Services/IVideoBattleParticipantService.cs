using System.Collections.Generic;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Enums;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface IVideoBattleParticipantService: IBaseEntityService<VideoBattleParticipant>
    {
        VideoBattleParticipant GetVideoBattleParticipant(int BattleId, int ParticipantId);

        VideoBattleParticipantStatus GetParticipationStatus(int BattleId, int ParticipantId);

        IList<VideoBattleParticipant> GetVideoBattleParticipants(int BattleId, VideoBattleParticipantStatus? ParticipantStatus);
    }
}