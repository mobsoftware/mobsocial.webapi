using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class CustomerCommentModel : BaseNopEntityModel
    {
        [Required]
        public int EntityId { get; set; }

        [Required]
        public string EntityName { get; set; }

        [Required]
        public string CommentText { get; set; }

        public string AdditionalData { get; set; }
    }
}
