using System.Collections.Generic;
using Mob.Core;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface ICustomerFollowService: IBaseEntityService<CustomerFollow>
    {
        CustomerFollow GetCustomerFollow<T>(int customerId, int entityId) where T : IFollowSupported;

        IList<CustomerFollow> GetCustomerFollows<T>(int customerId) where T : IFollowSupported;

        void Insert<T>(int customerId, int entityId) where T : IFollowSupported;

        void Delete<T>(int customerId, int entityId) where T : IFollowSupported;

        int GetFollowerCount<T>(int entityId) where T : IFollowSupported;

        IList<CustomerFollow> GetFollowers<T>(int entityId) where T : IFollowSupported;
    }
}