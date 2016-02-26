using System.Collections.Generic;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface IVideoBattleVoteService: IBaseEntityService<VideoBattleVote>
    {
        IList<VideoBattleVote> GetVideoBattleVotes(int VideoBattleId, int? UserId);
    }
}