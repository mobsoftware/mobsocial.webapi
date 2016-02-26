using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class SponsorTransactionModel : BaseNopModel
    {
        public int OrderId { get; set; }

        public string TransactionDate { get; set; }

        public decimal TransactionAmount { get; set; }

        public string TransactionAmountFormatted { get; set; }
    }
}
