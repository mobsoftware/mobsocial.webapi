using System;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class CustomerCommentPublicModel : BaseNopModel
    {
        public int Id { get; set; }

        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public string CommentText { get; set; }

        public string AdditionalData { get; set; }

        public bool CanDelete { get; set; }

        public int LikeCount { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsSpam { get; set; }

        public string CustomerName { get; set; }

        public string CustomerProfileUrl { get; set; }

        public string CustomerProfileImageUrl { get; set; }

    }
}
