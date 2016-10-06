using System;
using System.Linq;
using System.Web.Http;
using Nop.Core;
using Nop.Plugin.WebApi.MobSocial.Attributes;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Enums;
using Nop.Plugin.WebApi.MobSocial.Helpers;
using Nop.Plugin.WebApi.MobSocial.Models;
using Nop.Plugin.WebApi.MobSocial.Services;
using Nop.Services.Customers;

namespace Nop.Plugin.WebApi.MobSocial.Controllers
{
    [RoutePrefix("api/invitations")]
    [ApiAuthorize]
    public class InvitationApiController : BaseMobApiController
    {
        #region fields

        private readonly IInvitationService _invitationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerService _customerService;
        private readonly IMobSocialMessageService _mobSocialMessageService;
        #endregion

        #region ctor
        public InvitationApiController(IInvitationService invitationService, IWorkContext workContext, ICustomerService customerService, IMobSocialMessageService mobSocialMessageService, IStoreContext storeContext)
        {
            _invitationService = invitationService;
            _workContext = workContext;
            _customerService = customerService;
            _mobSocialMessageService = mobSocialMessageService;
            _storeContext = storeContext;
        }

        #endregion

        #region actions
        [Route("post")]
        [HttpPost]
        public IHttpActionResult Post(InvitationRequestModel requestModel)
        {
            if (!ModelState.IsValid)
                return
                    BadRequest();

            var currentUser = _workContext.CurrentCustomer;
            //send invitation to people who haven't been invited by this user
            //to improve performance, let's query all the invitations ever sent by user 
            var invitations = _invitationService.GetInvitationsByInviter(currentUser.Id);
            var toInviteList = requestModel.EmailAddress.Except(invitations.Select(x => x.InviteeEmailAddress));
            //invite them all
            var invitationUrl = InvitationHelpers.GetInvitationUrl();
            foreach (var email in toInviteList)
            {
                var invite = new Invitation() {
                    InviteeEmailAddress = email,
                    AcceptedOn = null,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    InviteStatus = InviteStatus.Sent
                };
                _invitationService.Insert(invite);
                _mobSocialMessageService.SendSomeoneInvitedYouToJoin(currentUser, email, email, invitationUrl,
                    _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id);
            }

            return Response(new { Success = true });
        } 
        #endregion
    }
}
