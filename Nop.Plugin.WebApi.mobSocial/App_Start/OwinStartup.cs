using System.Web.Http;
using Microsoft.Owin;
using Nop.Core.Infrastructure;
using Nop.Plugin.WebApi.mobSocial;
using Owin;

[assembly: OwinStartup(typeof(OwinStartup))]

namespace Nop.Plugin.WebApi.mobSocial
{
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {

            //new configuration for owin
            var config = new HttpConfiguration();
           
            //oauth
            ConfigureOAuth(app);

            //route registrations
            WebApiConfig.Register(config);
            
            //cors
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            
            //webapi
            app.UseWebApi(config);
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
           //TODO: Introduce oAuth authentication mechanism for api usage
        }
    }
}
