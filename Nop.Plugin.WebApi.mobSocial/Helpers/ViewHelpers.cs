using Nop.Plugin.WebApi.MobSocial.Constants;

namespace Nop.Plugin.WebApi.MobSocial.Helpers
{
    public static class ViewHelpers
    {
        public static string GetCorrectViewPath(string viewPath)
        {
            return "~/Plugins" + (MobSocialConstant.SuiteInstallation ? "/MobSocial.Suite" : "/WebApi.mobSocial") + "/" +
                   viewPath;
        }
    }
}