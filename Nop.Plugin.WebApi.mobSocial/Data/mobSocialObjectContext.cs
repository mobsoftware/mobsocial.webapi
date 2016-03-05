using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using Mob.Core.Data;
using Nop.Plugin.WebApi.MobSocial.Migrations;

namespace Nop.Plugin.WebApi.MobSocial.Data
{

    public class MobSocialObjectContext : MobDbContext
    {

        public MobSocialObjectContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var typesToRegister = typeof(MobSocialObjectContext).Assembly.GetTypes()
           .Where(type => !string.IsNullOrEmpty(type.Namespace))
           .Where(type => type.BaseType != null && type.BaseType.IsGenericType &&
               type.BaseType.GetGenericTypeDefinition() == typeof(BaseMobEntityTypeConfiguration<>));
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }

            base.OnModelCreating(modelBuilder);

        }

        public override void Install()
        {
            //It's required to set initializer to null (for SQL Server Compact).
            //otherwise, you'll get something like "The model backing the 'your context name' context has changed since the database was created. Consider using Code First Migrations to update the database"

            Database.SetInitializer<MobSocialObjectContext>(null);

            //because migrations are enabled, the install method may try to install tables which will eventually be installed by the migrations script
            //therefore commenting the install call below
            //base.Install();
        }

        public override void Uninstall()
        {
            try
            {
                // uninstall regardless of errors
                // Remove Url Records
                var dbScript = "DELETE FROM UrlRecord WHERE EntityName = 'Customer' OR EntityName = 'EventPage' OR EntityName = 'ArtistPage' OR EntityName = 'Song' OR EntityName = 'VideoBattle'; ";
                Database.ExecuteSqlCommand(dbScript);

                // DROP Tables via migrator. we just pass 0 to tell migrator to reset to original version
                var migrator = new DbMigrator(new Configuration());
                migrator.Update("0");

            }
            catch (Exception)
            {
                // ignored
            }

            base.Uninstall();


        }


    }

}








