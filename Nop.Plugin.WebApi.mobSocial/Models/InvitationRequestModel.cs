using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class InvitationRequestModel : BaseNopModel
    {
        public IList<string> EmailAddress { get; set; }

        public IList<int> UserIds { get; set; }
    }
}
