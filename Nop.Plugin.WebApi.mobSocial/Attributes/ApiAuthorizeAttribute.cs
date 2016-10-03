using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.WebApi.MobSocial.Attributes
{
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            if (workContext.CurrentCustomer.IsGuest())
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Unauthorized access");
                HttpContext.Current.Response.SuppressFormsAuthenticationRedirect = true;
            }
            else
            {
                base.OnAuthorization(actionContext);
            }
        }
    }
}