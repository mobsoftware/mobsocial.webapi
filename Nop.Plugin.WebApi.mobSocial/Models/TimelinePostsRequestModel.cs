using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class TimelinePostsRequestModel : BaseNopModel
    {
        public int CustomerId { get; set; }

        public int Count { get; set; }

        public int Page { get; set; }
    }
}
