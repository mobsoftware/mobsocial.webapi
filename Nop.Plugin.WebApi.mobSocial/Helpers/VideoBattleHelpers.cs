using System;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Enums;

namespace Nop.Plugin.WebApi.MobSocial.Helpers
{
    public static class VideoBattleHelpers
    {
        /// <summary>
        /// Returns remaining seconds of a battle for opening the battle (if it's pending) or completing the battle (if it's locked)
        /// </summary>
        /// <returns></returns>
        public static int GetRemainingSeconds(this VideoBattle videoBattle)
        {
            var now = DateTime.UtcNow;
            var endDate = DateTime.UtcNow;

            if (videoBattle.VideoBattleStatus == VideoBattleStatus.Pending && videoBattle.VotingStartDate > now)
            {
                endDate = videoBattle.VotingStartDate;
            }
            else if (videoBattle.VideoBattleStatus == VideoBattleStatus.Open)
            {
                endDate = videoBattle.VotingEndDate;
            }
            var diffDate = endDate.Subtract(now);
            var maxSeconds = Convert.ToInt32(diffDate.TotalSeconds);
            return maxSeconds;
        }
    }
}
