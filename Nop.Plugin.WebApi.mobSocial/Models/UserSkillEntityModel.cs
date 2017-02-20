using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class UserSkillEntityModel : BaseNopEntityModel
    {
        [Required]
        public string SkillName { get; set; }

        public int DisplayOrder { get; set; }

        public int UserId { get; set; }

        public string Description { get; set; }

        public string ExternalUrl { get; set; }

        public int[] MediaId { get; set; }

        public int UserSkillId { get; set; }

        public bool SystemSkill { get; set; }
    }
}