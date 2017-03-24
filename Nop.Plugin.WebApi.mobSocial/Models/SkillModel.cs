using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class SkillModel : BaseNopEntityModel
    {
        public string SkillName { get; set; }

        public int DisplayOrder { get; set; }

        public int UserId { get; set; }

        public string SeName { get; set; }

        public string Description { get; set; }
    }
}
