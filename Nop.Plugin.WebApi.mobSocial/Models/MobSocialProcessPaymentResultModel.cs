using Nop.Services.Payments;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class MobSocialProcessPaymentResultModel: BaseNopModel
    {
        public string PaymentMethodSystemName { get; set; }

        public ProcessPaymentResult ProcessPaymentResult { get; set; }
    }
}