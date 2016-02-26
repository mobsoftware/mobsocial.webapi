using System.Collections.Generic;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface IVideoBattleVideoService: IBaseEntityService<VideoBattleVideo>
    {
        VideoBattleVideo GetBattleVideo(int BattleId, int ParticipantId);

        IList<VideoBattleVideo> GetBattleVideos(int BattleId);
    }
}