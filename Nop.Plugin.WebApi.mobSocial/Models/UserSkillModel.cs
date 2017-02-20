using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class UserSkillModel : BaseNopEntityModel
    {
        public string SkillName { get; set; }

        public int DisplayOrder { get; set; }

        public int UserSkillId { get; set; }

        public string Description { get; set; }

        public CustomerProfilePublicModel User { get; set; }      

        public IList<MediaReponseModel> Media { get; set; }

        public string ExternalUrl { get; set; }

        public string FeaturedImageUrl { get; set; }

        public string SeName { get; set; }

        public int TotalMediaCount { get; set; }

        public int TotalVideoCount { get; set; }

        public int TotalPictureCount { get; set; }
    }
}