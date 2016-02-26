using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class InviteVotersModel : BaseNopModel
    {
        public int VideoBattleId { get; set; }

        public IList<int> VoterIds { get; set; }

        public IList<string> Emails { get; set; } 
    }
}
