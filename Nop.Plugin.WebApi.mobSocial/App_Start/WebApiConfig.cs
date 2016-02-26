using System.Web.Mvc;
using System.Web.Http;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.WebApi;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.WebApi.mobSocial
{
    public class WebApiConfig
    {

        public static void Register(HttpConfiguration configuration)
        {
            //setup attribute routes
            configuration.MapHttpAttributeRoutes();

            //remove xml responses
            //TODO: MAKE IT CONFIGURABLE
            configuration.Formatters.Remove(configuration.Formatters.XmlFormatter);

            configuration.DependencyResolver = new AutofacWebApiDependencyResolver(EngineContext.Current.ContainerManager.Container);
        }

       

    }
}