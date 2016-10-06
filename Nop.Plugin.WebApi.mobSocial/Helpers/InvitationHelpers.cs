using System;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.WebApi.MobSocial.Helpers
{
    public class InvitationHelpers
    {
        public static string GetInvitationUrl()
        {
            var currentCustomer = EngineContext.Current.Resolve<IWorkContext>().CurrentCustomer;
            var currentStore = EngineContext.Current.Resolve<IStoreContext>().CurrentStore;
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

            var storeUrl = currentStore.SslEnabled ? currentStore.SecureUrl : currentStore.Url;
            var registrationUrl = string.Join("", storeUrl, urlHelper.RouteUrl("Register"));

            var uriBuilder = new UriBuilder(registrationUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["ref"] = currentCustomer.Id.ToString();
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }
    }
}
