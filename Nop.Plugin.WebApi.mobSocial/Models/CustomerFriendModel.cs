using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class CustomerFriendModel : BaseNopModel
    {


        public string CustomerDisplayName { get; set; }

        public string ProfileUrl { get; set; }

        public string ProfileThumbnailUrl { get; set; }
    }
}