using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class SkillModel : BaseNopEntityModel
    {
        public string SkillName { get; set; }

        public string Description { get; set; }

        public int CustomerId { get; set; }

        public int DisplayOrder { get; set; }
    }
}
