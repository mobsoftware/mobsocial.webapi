﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mob.Core.Data;
using Mob.Core.Migrations;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Plugin.WebApi.MobSocial.Constants;
using Nop.Plugin.WebApi.MobSocial.Data;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Services;
using Nop.Services.Configuration;
using Nop.Services.Messages;

namespace Nop.Plugin.WebApi.MobSocial.Migrations
{
    public class MobSocialMigration : IStartupTask
    {
       
        public void RunCustomMigration()
        {
            //here we include the code that'll run each time something needs to be upgraded to existing installation
            //we keep track of upgrades by version numbers
            //so first get the version number

            //only if plugin is installed obviously
            var pluginFinder = EngineContext.Current.Resolve<IPluginFinder>();

            var pluginDescriptor = pluginFinder.GetPluginDescriptorBySystemName("WebApi.mobSocial");

            if (pluginDescriptor == null || !pluginDescriptor.Installed)
            {
                return;
            }
            
            var _settingService = EngineContext.Current.Resolve<ISettingService>();

            var settings = _settingService.LoadSetting<mobSocialSettings>();

            if (settings.Version >= MobSocialConstant.ReleaseVersion)
                return; // no need for any upgrade

            if (settings.Version < 1.1m)
            {
                var storeContext = EngineContext.Current.Resolve<IStoreContext>();
                var emailAccountSettings = EngineContext.Current.Resolve<EmailAccountSettings>();
                var messageTemplateService = EngineContext.Current.Resolve<IMessageTemplateService>();
                var someInvitedToJoinNotification = new MessageTemplate() {
                    Name = "MobSocial.InvitationToJoinNotification",
                    Subject = "%Customer.FirstName% has invited you to join " + storeContext.CurrentStore.Name,
                    Body = "Hi, " + "%Customer.FirstName% has invited you to join " + storeContext.CurrentStore.Name + " <a href='%Invitation.Url%'>Click here</a> to join.",
                    EmailAccountId = emailAccountSettings.DefaultEmailAccountId,
                    IsActive = true,
                    LimitedToStores = false
                };

                messageTemplateService.InsertMessageTemplate(someInvitedToJoinNotification);
            }
           

            //and update the setting
            settings.Version = MobSocialConstant.ReleaseVersion;
            _settingService.SaveSetting(settings);
            _settingService.ClearCache();
            
           
        }

        public void Execute()
        {
            RunCustomMigration();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
