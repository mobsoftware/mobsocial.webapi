using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.WebApi.MobSocial.Enums;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class PurchasePassModel : BaseNopModel
    {

        public int CustomerPaymentMethodId { get; set; }

        public int AvailableOrderId { get; set; }

        public decimal Amount { get; set; }

        public int BattleId { get; set; }

        public BattleType BattleType { get; set; }

        public CustomerPaymentRequestModel CustomerPaymentRequest { get; set; }

        public PurchaseType PurchaseType { get; set; }
    }
}
