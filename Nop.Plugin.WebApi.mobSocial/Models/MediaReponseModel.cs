using System;
using Nop.Plugin.WebApi.MobSocial.Enums;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class MediaReponseModel : BaseNopModel
    {
        public string Url { get; set; }

        public string ThumbnailUrl { get; set; }

        public MediaType MediaType { get; set; }

        public string MimeType { get; set; }

        public int Id { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateCreatedLocal { get; set; }

        public CustomerProfilePublicModel CreatedBy { get; set; }

        public int TotalLikes { get; set; }

        public int TotalComments { get; set; }

        public bool CanComment { get; set; }

        public int LikeStatus { get; set; }

        public int NextMediaId { get; set; }

        public int PreviousMediaId { get; set; }

        public bool FullyLoaded { get; set; }

    }
}