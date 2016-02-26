using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mob.Core.Data;
using Mob.Core.Services;
using Nop.Core;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Services.Seo;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public class VideBattlePrizeService: BaseEntityService<VideoBattlePrize>, IVideoBattlePrizeService
    {
 
        public VideBattlePrizeService(IMobRepository<VideoBattlePrize> repository) : base(repository)
        {
        }

        public override List<VideoBattlePrize> GetAll(string Term, int Count = 15, int Page = 1)
        {
            throw new NotImplementedException();
        }

        public IList<VideoBattlePrize> GetBattlePrizes(int VideoBattleId)
        {
            return Repository.Table.Where(x => x.VideoBattleId == VideoBattleId).OrderBy(x => x.WinnerPosition).ToList();
        }
    }
}
