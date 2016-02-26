using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mob.Core.Data;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Data
{
    public class CustomerPaymentMethodMap :  BaseMobEntityTypeConfiguration<CustomerPaymentMethod>
    {
        public CustomerPaymentMethodMap()
        {
            Property(x => x.CardIssuerType);
            Property(x => x.CardNumber);
            Property(x => x.CardNumberMasked);
            Property(x => x.CustomerId);
            Property(x => x.ExpireMonth);
            Property(x => x.ExpireYear);
            Property(x => x.NameOnCard);
            Property(x => x.PaymentMethod);
        }
    }
}
