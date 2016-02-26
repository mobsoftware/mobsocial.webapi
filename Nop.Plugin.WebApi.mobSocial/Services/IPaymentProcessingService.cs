using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Models;
using Nop.Services.Payments;

namespace Nop.Plugin.WebApi.mobSocial.Services
{
    public interface IPaymentProcessingService
    {
        MobSocialProcessPaymentResultModel ProcessPayment(Customer Customer, CustomerPaymentMethod PaymentMethod, decimal PaymentAmount, bool AuthorizeOnly = false);

        CapturePaymentResult CapturePayment(Order order);

        VoidPaymentResult VoidPayment(Order order);

        decimal GetNetAmountAfterPaymentProcessing(decimal Amount);
    }
}