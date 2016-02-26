using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Customer;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class FeaturedVideosModel : BaseNopModel
    {
        public FeaturedVideosModel()
        {
            FeaturedVideos = new List<FeaturedVideoModel>();
        }

        public List<FeaturedVideoModel> FeaturedVideos { get; set; }

    }

    public class FeaturedVideoModel : BaseNopModel
    {

        public string CustomerProfileUrl { get; set; }
        public string ThumbnailUrl { get; set; }

    }

}