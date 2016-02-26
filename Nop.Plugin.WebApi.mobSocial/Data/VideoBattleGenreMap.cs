using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mob.Core.Data;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Data
{
    public class VideoBattleGenreMap : BaseMobEntityTypeConfiguration<VideoBattleGenre>
    {
        public VideoBattleGenreMap()
        {
            ToTable("VideoBattleGenre");
            HasKey(x => x.Id);
            Property(x => x.VideoBattleId);
            Property(x => x.VideoGenreId);
        }
    }
}
