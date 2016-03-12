using System;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class TimelinePostDisplayModel : BaseNopModel
    {
        public int Id { get; set; }

        public int OwnerId { get; set; }

        public string OwnerEntityType { get; set; }

        public string OwnerImageUrl { get; set; }

        public string OwnerProfileUrl { get; set; }

        public string OwnerName { get; set; }

        public string PostTypeName { get; set; }

        public bool IsSponsored { get; set; }

        public string Message { get; set; }

        public string AdditionalAttributeValue { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public int TotalLikes { get; set; }

        public int TotalComments { get; set; }
    }
}
