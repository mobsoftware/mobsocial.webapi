using System.Collections;
using System.Collections.Generic;
using Mob.Core.Services;
using Nop.Core.Domain.Customers;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface ICustomerProfileService : IBaseEntityService<CustomerProfile>
    {
        int GetFriendCount(int customerId);

        IList<Customer> SearchCustomers(string searchText, int[] customerRoleIds, bool excludeLoggedInUser = true,
            bool excludeHiddenProfiles = true, int count = 15, int page = 1);
    }
}