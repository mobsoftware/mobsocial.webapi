using System.Web.Http.Routing;
using System.Web.Routing;

namespace Nop.Plugin.WebApi.MobSocial.Extensions
{
    public static class UrlExtensions
    {
        /// <summary>
        /// Retrieves a particular routeurl from the global route table. Because the app is running inside OWIN host, we need to query route from the global routetable 
        /// instead of route from OWIN configuration
        /// </summary>
        /// <returns>The url of route</returns>
        public static string RouteUrl(this UrlHelper urlHelper, string routeName, RouteValueDictionary routeValues)
        {
           var vPath = RouteTable.Routes.GetVirtualPath(null, routeName, routeValues);
            
            if (vPath == null)
                return string.Empty;
            return vPath.VirtualPath;
        }
    }
}
