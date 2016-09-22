using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Plugin.WebApi.MobSocial;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Models;
using Nop.Plugin.WebApi.MobSocial.Services;
using Nop.Services.Payments;

namespace Nop.Plugin.WebApi.mobSocial.Services
{
    public class PaymentProcessingService : IPaymentProcessingService
    {
        const decimal MicroMacroPaymentSwitchingAmount = 11; //< $11.00 micro, >$11.0 macro

        private readonly mobSocialSettings _mobSocialSettings;
        private readonly IPluginFinder _pluginFinder;
        private readonly IMobSecurityService _mobSecurityService;
        private readonly IStoreContext _storeContext;

        public PaymentProcessingService(mobSocialSettings mobSocialSettings, IPluginFinder pluginFinder, IMobSecurityService mobSecurityService, IStoreContext storeContext)
        {
            _mobSocialSettings = mobSocialSettings;
            _pluginFinder = pluginFinder;
            _mobSecurityService = mobSecurityService;
            _storeContext = storeContext;
        }

        private IPaymentMethod GetPluginInstance(string pluginSystemName)
        {
            var isInstalled = false;
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(pluginSystemName);

            if (pluginDescriptor != null && pluginDescriptor.Installed)
            {
                isInstalled = true;
            }

            //is installed
            if (!isInstalled)
                return null;

            var plugin = pluginDescriptor.Instance();
            //can't find the plugin
            return plugin as IPaymentMethod;

        }
        public MobSocialProcessPaymentResultModel ProcessPayment(Customer Customer, CustomerPaymentMethod PaymentMethod, decimal PaymentAmount, bool AuthorizeOnly = false)
        {
            //check if the plugin is installed and activated
            //depending on the payment amount, we may wish to switch between macro and micro payments
            //check if plugin is installed or not
            //TODO: Change the other payment method to appropriate one for macro payments
            var paymentPluginSystemName = PaymentAmount < MicroMacroPaymentSwitchingAmount ? "Payments.PayPalDirect" : "Payments.PayPalDirect";

            //if we get an instance, then the plugin is installed
            var plugin = GetPluginInstance(paymentPluginSystemName);

            //plugin should be available and if it's an authorization transaction, the plugin should actually support authorize and capture
            if (plugin == null || (!plugin.SupportCapture && AuthorizeOnly))
                return null;

            //default to first billing address
            Customer.BillingAddress = Customer.BillingAddress ?? Customer.Addresses.First();

            var key = _mobSecurityService.GetSavedEncryptionKey();
            var salt = _mobSecurityService.GetSavedSalt();

            //decrypt card number stored for processing
            var cardNumber = _mobSecurityService.Decrypt(PaymentMethod.CardNumber, key, salt); //decrypt the card info

            //now create a payment processing request
            var paymentProcessingRequest = new ProcessPaymentRequest()
            {
                CustomerId = Customer.Id,
                OrderTotal = PaymentAmount,
                CreditCardName = PaymentMethod.NameOnCard,
                CreditCardExpireMonth = int.Parse(PaymentMethod.ExpireMonth),
                CreditCardExpireYear = int.Parse(PaymentMethod.ExpireYear),
                PaymentMethodSystemName = paymentPluginSystemName,
                CreditCardType = PaymentMethod.CardIssuerType,
                CreditCardNumber = cardNumber,
                StoreId = _storeContext.CurrentStore.Id
            };

            var resultModel = new MobSocialProcessPaymentResultModel()
            {
                ProcessPaymentResult = plugin.ProcessPayment(paymentProcessingRequest),
                PaymentMethodSystemName = paymentPluginSystemName
            };
            return resultModel;
        }

        public CapturePaymentResult CapturePayment(Order order)
        {
            var plugin = GetPluginInstance(order.PaymentMethodSystemName);
            //plugin should be available and if it's an authorization transaction, the plugin should actually support authorize and capture/void
            if (plugin == null || !plugin.SupportCapture)
                return null;

            return plugin.Capture(new CapturePaymentRequest()
            {
                Order = order
            });
        }

        public VoidPaymentResult VoidPayment(Order order)
        {
            var plugin = GetPluginInstance(order.PaymentMethodSystemName);
            //plugin should be available and if it's an authorization transaction, the plugin should actually support authorize and capture/void
            if (plugin == null || !plugin.SupportVoid)
                return null;

            return plugin.Void(new VoidPaymentRequest() {
                Order = order
            });
        }

        public decimal GetNetAmountAfterPaymentProcessing(decimal Amount)
        {
            var fixProcessingFee = Amount < MicroMacroPaymentSwitchingAmount
                ? _mobSocialSettings.MicroPaymentsFixedPaymentProcessingFee
                : _mobSocialSettings.MacroPaymentsFixedPaymentProcessingFee;

            var processingPercentage = Amount < MicroMacroPaymentSwitchingAmount
                ? _mobSocialSettings.MicroPaymentsPaymentProcessingPercentage
                : _mobSocialSettings.MacroPaymentsPaymentProcessingPercentage;

            return Amount - fixProcessingFee - (Amount * processingPercentage / 100);
        }
    }
}