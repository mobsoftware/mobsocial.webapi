using Mob.Core.Data;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Data
{
    public class InvitationMap : BaseMobEntityTypeConfiguration<Invitation>
    {
        public InvitationMap()
        {
            Property(x => x.InviterUserId);
            Property(x => x.InviteeEmailAddress);
            Property(x => x.InviteeName);
            Property(x => x.InviteStatus);
            Property(x => x.AcceptedOn);
        }
    }
}
