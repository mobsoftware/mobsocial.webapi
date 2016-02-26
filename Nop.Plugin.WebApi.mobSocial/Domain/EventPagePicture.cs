using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Seo;
using System;
using System.Collections.Generic;
using Mob.Core.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Domain
{
    public class EventPagePicture : BaseMobPictureEntity
    {
        public virtual EventPage EventPage { get; set; }

    }
}