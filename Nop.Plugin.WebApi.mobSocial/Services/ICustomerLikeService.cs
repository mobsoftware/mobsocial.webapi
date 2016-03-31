using System.Collections.Generic;
using Mob.Core;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface ICustomerLikeService: IBaseEntityService<CustomerLike>
    {
        CustomerLike GetCustomerLike<T>(int customerId, int entityId);

        IList<CustomerLike> GetCustomerLikes<T>(int customerId);

        void Insert<T>(int customerId, int entityId);

        void Delete<T>(int customerId, int entityId);

        int GetLikeCount<T>(int entityId);

        IList<CustomerLike> GetEntityLikes<T>(int entityId);
    }
}