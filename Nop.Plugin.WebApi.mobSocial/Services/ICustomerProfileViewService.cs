using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface ICustomerProfileViewService : IBaseEntityService<CustomerProfileView>
    {
        int GetViewCount(int customerId);
    }
}