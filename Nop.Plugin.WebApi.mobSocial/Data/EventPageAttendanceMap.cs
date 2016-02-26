using System.Data.Entity.ModelConfiguration;
using Mob.Core.Data;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Data
{

    public class EventPageAttendanceMap : BaseMobEntityTypeConfiguration<EventPageAttendance>
    {

        public EventPageAttendanceMap()
        {
            //Map the additional properties
            Property(m => m.EventPageId);
            Property(m => m.CustomerId);
            Property(m => m.AttendanceStatusId);

            
        }

    }
}
