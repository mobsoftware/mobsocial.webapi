namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface IOAuthCredentials
    {
        string ConsumerKey { get; set; }
        string ConsumerSecret { get; set; }
    }
}
