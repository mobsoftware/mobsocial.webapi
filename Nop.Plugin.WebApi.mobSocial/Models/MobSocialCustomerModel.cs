using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class MobSocialCustomerModel : BaseNopModel
    {
        public int CustomerId { get; set; }

        public bool HideProfile { get; set; }

    }
}