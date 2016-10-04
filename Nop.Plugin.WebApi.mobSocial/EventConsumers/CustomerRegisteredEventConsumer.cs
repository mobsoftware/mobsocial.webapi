using System;
using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.Plugin.WebApi.MobSocial.Enums;
using Nop.Plugin.WebApi.MobSocial.Events;
using Nop.Plugin.WebApi.MobSocial.Services;
using Nop.Services.Customers;
using Nop.Services.Events;

namespace Nop.Plugin.WebApi.MobSocial.EventConsumers
{
    public class CustomerRegisteredEventConsumer : IConsumer<CustomerRegisteredEvent>
    {
        private readonly IInvitationService _invitationService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICustomerService _customerService;

        public CustomerRegisteredEventConsumer(IInvitationService invitationService, IEventPublisher eventPublisher, ICustomerService customerService)
        {
            _invitationService = invitationService;
            _eventPublisher = eventPublisher;
            _customerService = customerService;
        }

        public void HandleEvent(CustomerRegisteredEvent eventMessage)
        {
            //let's see if we need to perform any updates to invites if current registered customer is indeed an invited one
            //this should usually be one update unless a user has been invited by more than one users
            var customer = eventMessage.Customer;
            var invitations = _invitationService.GetInvitationsByInvitee(customer.Email);
            var allInviters = _customerService.GetCustomersByIds(invitations.Select(x => x.InviterUserId).ToArray());
            foreach (var invite in invitations)
            {
                invite.AcceptedOn = DateTime.UtcNow;
                invite.InviteStatus = InviteStatus.Accepted;
                invite.DateUpdated = DateTime.UtcNow;
                _invitationService.Update(invite);

                //publish the invitation accepted event to capture in other plugins
                var inviter = allInviters.First(x => x.Id == invite.InviterUserId);
                var invitationAcceptedEvent = new InvitationAcceptedEvent(inviter, customer);
                _eventPublisher.Publish(invitationAcceptedEvent);
            }
        }
    }
}