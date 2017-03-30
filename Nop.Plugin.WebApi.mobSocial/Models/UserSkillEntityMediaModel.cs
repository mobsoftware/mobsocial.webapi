using Nop.Plugin.WebApi.MobSocial.Enums;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class UserSkillEntityMediaModel : BaseNopModel
    {
        public int UserSkillId { get; set; }

        public int MediaId { get; set; }
    }
}