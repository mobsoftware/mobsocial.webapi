using System.Data.Entity.Infrastructure;
using System.Reflection;
using Mob.Core.Migrations;
using Nop.Core.Data;
using Nop.Plugin.WebApi.MobSocial.Data;
using Nop.Plugin.WebApi.MobSocial.Helpers;

namespace Nop.Plugin.WebApi.MobSocial.Migrations
{
    internal sealed class Configuration : MobMigrationConfiguration<MobSocialObjectContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;

            AutomaticMigrationDataLossAllowed = true;

            MigrationsAssembly = Assembly.GetExecutingAssembly();
            MigrationsNamespace = "Nop.Plugin.WebApi.MobSocial.Migrations";

            //specify database so that it doesn't throw error during migration. Otherwise, for some reasons it defaults to sqlce and gives error 
            var dataSettingsManager = new DataSettingsManager();
            var dataSettings = dataSettingsManager.LoadSettings();
            TargetDatabase = new DbConnectionInfo(dataSettings.DataConnectionString, DatabaseHelpers.GetDbProviderName(dataSettings.DataProvider));

        }
    }
}
