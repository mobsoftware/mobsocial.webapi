using System;
using System.Linq;
using System.Web;
using Mob.Core.Extensions;
using Nop.Core.Domain.Customers;
using Nop.Plugin.WebApi.MobSocial.Domain;
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

            //in case this customer has registered with a different email than the one on which invitation was sent, it's better to check
            //for ref attribute to see if there is the userid of the inviter
            if (string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ref"]) || invitations.Any())
                return;

            var refId = HttpContext.Current.Request.QueryString["ref"];
            var userId = refId.IsInteger() ? Convert.ToInt32(refId) : 0;
            if (userId != 0)
            {
                var inviter = _customerService.GetCustomerById(userId);
                if (inviter == null)
                    return;
                //we'll have to make a new invitation entry and assign it to the userId (inviter that is)
                var invite = new Invitation()
                {
                    DateCreated = DateTime.UtcNow,
                    AcceptedOn = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    InviteeName = customer.GetFullName(),
                    InviteeEmailAddress = customer.Email,
                    InviteStatus = InviteStatus.Accepted,
                    InviteeUserId = customer.Id,
                    InviterUserId = userId
                };
                _invitationService.Insert(invite);
                //publish event
                var invitationAcceptedEvent = new InvitationAcceptedEvent(inviter, customer);
                _eventPublisher.Publish(invitationAcceptedEvent);
            }
        }
    }
}