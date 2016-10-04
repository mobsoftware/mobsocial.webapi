using Nop.Core.Domain.Customers;

namespace Nop.Plugin.WebApi.MobSocial.Events
{
    public class InvitationAcceptedEvent
    {
        public Customer Inviter { get; private set; }

        public Customer Invitee { get; private set; }

        public InvitationAcceptedEvent(Customer inviter, Customer invitee)
        {
            Inviter = inviter;
            Invitee = invitee;
        }
    }
}