using System;
using Mob.Core.Domain;
using Nop.Plugin.WebApi.MobSocial.Enums;

namespace Nop.Plugin.WebApi.MobSocial.Domain
{
    public class Invitation : BaseMobEntity
    {
        public int InviterUserId { get; set; }

        public string InviteeEmailAddress { get; set; }

        public string InviteeName { get; set; }

        public InviteStatus InviteStatus { get; set; }

        public DateTime? AcceptedOn { get; set; }
    }
}
