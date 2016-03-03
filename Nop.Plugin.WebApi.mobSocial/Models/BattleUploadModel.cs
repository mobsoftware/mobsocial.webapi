using Nop.Plugin.WebApi.MobSocial.MediaFormatters.Infrastructure;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class BattleUploadModel : BaseNopModel
    {
        public int BattleId { get; set; }

        public int ParticipantId { get; set; }

        public HttpFile File { get; set; }
    }
}
