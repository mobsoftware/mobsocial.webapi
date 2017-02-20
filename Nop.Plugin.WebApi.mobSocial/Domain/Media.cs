using System;
using Mob.Core.Domain;
using Nop.Plugin.WebApi.MobSocial.Enums;

namespace Nop.Plugin.WebApi.MobSocial.Domain
{
    public class Media : BaseMobEntity
    {
        public string Name { get; set; }

        public string SystemName { get; set; }

        public string Description { get; set; }

        public string AlternativeText { get; set; }

        public string LocalPath { get; set; }

        public string ThumbnailPath { get; set; }

        public string MimeType { get; set; }

        public byte[] Binary { get; set; }

        public MediaType MediaType { get; set; }

        public int UserId { get; set; }

        public DateTime DateCreated { get; set; }
    }
}