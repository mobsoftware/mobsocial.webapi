using Mob.Core.Data;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Data
{
    public class SponsorPassMap : BaseMobEntityTypeConfiguration<SponsorPass>
    {
        public SponsorPassMap()
        {
            Property(x => x.CustomerId);
            Property(x => x.SponsorPassOrderId);
            Property(x => x.Status);
            Property(x => x.BattleId);
            Property(x => x.BattleType);
        }
    }
}