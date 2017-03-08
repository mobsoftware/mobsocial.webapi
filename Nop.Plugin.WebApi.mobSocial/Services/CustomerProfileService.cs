using System;
using System.Collections.Generic;
using System.Linq;
using Mob.Core.Data;
using Mob.Core.Services;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public class CustomerProfileService : BaseEntityService<CustomerProfile>, ICustomerProfileService
    {
        private readonly IMobRepository<CustomerFriend> _customerFriendRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<GenericAttribute> _gaRepository;
        private readonly IWorkContext _workContext;
        public CustomerProfileService(IMobRepository<CustomerProfile> customerProfileRepository,
            IMobRepository<CustomerFriend> customerFriendRepository, IRepository<Customer> customerRepository, IRepository<GenericAttribute> gaRepository, IWorkContext workContext) 
            : base(customerProfileRepository)
        {
            _customerFriendRepository = customerFriendRepository;
            _customerRepository = customerRepository;
            _gaRepository = gaRepository;
            _workContext = workContext;
        }

        public CustomerProfile GetByCustomerId(int customerId) {
            return base.Repository.Table.FirstOrDefault(x => x.CustomerId == customerId);
        }

        public int GetFriendCount(int customerId)
        {
            return _customerFriendRepository
                    .Table
                    .Count(x => (x.FromCustomerId == customerId || x.ToCustomerId == customerId) &&
                                 x.Confirmed);

        }

        public IList<Customer> SearchCustomers(string searchText, int[] customerRoleIds, bool excludeLoggedInUser = true, bool excludeHiddenProfiles = true, int count = 15, int page = 1)
        {
            var query = _customerRepository.Table;
            if (excludeLoggedInUser)
            {
                query = query.Where(x => x.Id != _workContext.CurrentCustomer.Id);
            }

            if (!String.IsNullOrWhiteSpace(searchText))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == "Customer" &&
                        (z.Attribute.Key == SystemCustomerAttributeNames.FirstName || z.Attribute.Key == SystemCustomerAttributeNames.LastName) &&
                        z.Attribute.Value.Contains(searchText))
                    .Select(z => z.Customer);
            }
            if (excludeHiddenProfiles)
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new {Customer = x, Attribute = y})
                    .Where(z => z.Attribute.KeyGroup == "Customer" &&
                                ((z.Attribute.Key == "hideProfile" && z.Attribute.Value == "False") ||
                                 _gaRepository.Table.Count(x => x.KeyGroup == "Customer" && x.Key == "hideProfile" && x.EntityId == z.Customer.Id) == 0))
                    .Select(z => z.Customer);
            }
            if (customerRoleIds != null && customerRoleIds.Length > 0)
                query = query.Where(c => c.CustomerRoles.Select(cr => cr.Id).Intersect(customerRoleIds).Any());

            query = query.Distinct().OrderBy(x => x.Id).Skip((page - 1)*count).Take(count);
            return query.ToList();
        }

        public override List<CustomerProfile> GetAll(string Term, int Count = 15, int Page = 1)
        {
            throw new NotImplementedException();
        }
    }

}
