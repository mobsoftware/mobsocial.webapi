using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Integration.WebApi;
using Mob.Core;
using Mob.Core.Migrations;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.WebApi.MobSocial.Data;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Migrations;
using Nop.Plugin.WebApi.MobSocial.Services;
using Nop.Services.Media;
using Nop.Services.Seo;
using SitemapGenerator = Nop.Plugin.WebApi.MobSocial.Services.SitemapGenerator;
using Nop.Core.Configuration;
using Nop.Core.Plugins;
using Nop.Plugin.WebApi.mobSocial;
using Nop.Plugin.WebApi.mobSocial.Services;
using Nop.Plugin.WebApi.MobSocial.Controllers;

namespace Nop.Plugin.WebApi.MobSocial
{
    public class DependencyRegistrar : BaseMobDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_social_network";


        public override void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig NopConfig)
        {
           

            //Load custom data settings
            var dataSettingsManager = new DataSettingsManager();
            DataSettings dataSettings = dataSettingsManager.LoadSettings();

            //Register custom object context
            builder.Register<IDbContext>(c => RegisterIDbContext(c, dataSettings)).Named<IDbContext>(CONTEXT_NAME).InstancePerRequest();
            builder.Register(c => RegisterIDbContext(c, dataSettings)).InstancePerRequest();

            var pluginFinderTypes = typeFinder.FindClassesOfType<IPluginFinder>();

            var isInstalled = false;

            foreach (var pluginFinderType in pluginFinderTypes)
            {
                var pluginFinder = Activator.CreateInstance(pluginFinderType) as IPluginFinder;
                var pluginDescriptor = pluginFinder.GetPluginDescriptorBySystemName("WebApi.mobSocial");

                if (pluginDescriptor != null && pluginDescriptor.Installed)
                {
                    isInstalled = true;
                    break;
                }
            }
            
            //register api controllers
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

           

            //Register services
            builder.RegisterType<MobSocialService>().As<IMobSocialService>();
            builder.RegisterType<ArtistPageAPIService>().As<IArtistPageAPIService>();
     
            //builder.RegisterType<MobSocialPictureService>().As<IPictureService>().InstancePerRequest();
            builder.RegisterType<MobSocialMessageService>().As<IMobSocialMessageService>().InstancePerRequest();
            builder.RegisterType<CustomerAlbumPictureService>().As<ICustomerAlbumPictureService>().InstancePerRequest();
            builder.RegisterType<CustomerVideoAlbumService>().As<ICustomerVideoAlbumService>().InstancePerRequest();

            builder.RegisterType<CustomerProfileViewService>().As<CustomerProfileViewService>().InstancePerRequest();
            builder.RegisterType<CustomerProfileService>().As<CustomerProfileService>().InstancePerRequest();
            builder.RegisterType<EchoNestMusicService>().As<IMusicService>().InstancePerRequest();
            builder.RegisterType<MusicApiCredentials>().As<IOAuthCredentials>().InstancePerRequest();
            builder.RegisterType<MusicApiUri>().As<IApiUri>().InstancePerRequest();
            builder.RegisterType<OAuthService>().As<IOAuthService>().InstancePerRequest();

            builder.RegisterType<MobSecurityService>().As<IMobSecurityService>().InstancePerRequest();
            builder.RegisterType<PaymentProcessingService>().As<IPaymentProcessingService>().InstancePerRequest();
            builder.RegisterType<VoterPassService>().As<IVoterPassService>().InstancePerRequest();
            builder.RegisterType<TimelineAutoPublisher>().As<ITimelineAutoPublisher>().InstancePerRequest();
            // Override any NopCommerce Services below:
            builder.RegisterType<SitemapGenerator>().As<Nop.Services.Seo.ISitemapGenerator>().InstancePerLifetimeScope();

            //call the core registrar
            base.Register(builder, typeFinder, NopConfig);

            if (isInstalled)
            {
                //db migrations, lets update if needed
                var migrator = new DbMigrator(new Configuration());
                migrator.Update();
            }
            
        }
        
        /// <summary>
        /// Registers the I db context.
        /// </summary>
        /// <param name="componentContext">The component context.</param>
        /// <param name="dataSettings">The data settings.</param>
        /// <returns></returns>
        private MobSocialObjectContext RegisterIDbContext(IComponentContext componentContext, DataSettings dataSettings)
        {
            string dataConnectionStrings;

            if (dataSettings != null && dataSettings.IsValid())
                dataConnectionStrings = dataSettings.DataConnectionString;
            else
                dataConnectionStrings = componentContext.Resolve<DataSettings>().DataConnectionString;

            return new MobSocialObjectContext(dataConnectionStrings);

        }


        public int Order
        {
            get { return 1; }
        }

        public override string ContextName
        {
            get { return CONTEXT_NAME; }
        }
    }
}
