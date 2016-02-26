using System.Collections.Generic;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    /// <summary>
    /// Product service
    /// </summary>
    public interface IEventPageHotelService : IBaseEntityService<EventPageHotel>
    {
        List<EventPageHotel> GetAll(int eventPageId);
    }

}
