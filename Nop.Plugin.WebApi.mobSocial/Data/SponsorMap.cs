using Mob.Core.Data;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Data
{
    public class SponsorMap : BaseMobEntityTypeConfiguration<Sponsor>
    {
        public SponsorMap()
        {
            Property(x => x.BattleId);
            Property(x => x.BattleType);
            Property(x => x.CustomerId);
            Property(x => x.SponsorshipAmount);
            Property(x => x.SponsorshipStatus);
            Property(x => x.SponsorshipType);
        }
    }
}