using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class SetFeaturedMediaModel : BaseNopModel
    {
        public int SkillId { get; set; }

        public int MediaId { get; set; }
    }
}