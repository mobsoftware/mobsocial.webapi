namespace Nop.Plugin.WebApi.MobSocial.Helpers
{
    public class DatabaseHelpers
    {
        public static string GetDbProviderName(string nopDbProviderName)
        {
            switch (nopDbProviderName)
            {
                case "sqlserver":
                    return "System.Data.SqlClient";
                case "sqlce":
                    return "System.Data.SqlServerCe.4.0";
            }
            return string.Empty;
        }
    }
}
