using Nop.Plugin.WebApi.MobSocial.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mob.Core.Data;

namespace Nop.Plugin.WebApi.MobSocial.Data
{
    public class SharedSongMap : BaseMobEntityTypeConfiguration<SharedSong>
    {
        public SharedSongMap()
        {
            Property(x => x.SenderId);
            Property(x => x.CustomerId);
            Property(x => x.Message);

            Property(x => x.SharedOn);
            Property(x => x.SongId);
        }
    }
}
