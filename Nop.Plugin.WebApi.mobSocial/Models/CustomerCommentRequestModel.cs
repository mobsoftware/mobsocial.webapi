using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class CustomerCommentRequestModel: BaseNopModel
    {
        [Required]
        public string EntityName { get; set; }

        [Required]
        public int EntityId { get; set; }

        public int Page { get; set; }

        public int Count { get; set; }
    }
}
