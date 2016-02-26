using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mob.Core.Migrations;
using Nop.Plugin.WebApi.MobSocial.Data;

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

        }
    }
}
