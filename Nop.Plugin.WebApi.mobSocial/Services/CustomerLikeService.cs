using System;
using System.Collections.Generic;
using System.Linq;
using Mob.Core;
using Mob.Core.Data;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Services.Customers;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public class CustomerLikeService : BaseEntityService<CustomerLike>, ICustomerLikeService
    {
        public CustomerLikeService(IMobRepository<CustomerLike> repository) : base(repository)
        {
        }

        public override List<CustomerLike> GetAll(string Term, int Count = 15, int Page = 1)
        {
            return Repository.Table.ToList();
        }

        public CustomerLike GetCustomerLike<T>(int customerId, int entityId)
        {
            return
                Repository.Table.FirstOrDefault(
                    x => x.EntityId == entityId && x.CustomerId == customerId && x.EntityName == typeof(T).Name);
        }

        public IList<CustomerLike> GetCustomerLikes<T>(int customerId)
        {
            return
                Repository.Table.Where(x => x.CustomerId == customerId && x.EntityName == typeof(T).Name)
                    .ToList();
        }

       
        public void Insert<T>(int customerId, int entityId)
        {
            //insert only if required
            if (
                !Repository.Table.Any(
                    x =>
                        x.EntityId == entityId && x.CustomerId == customerId &&
                        x.EntityName == typeof (T).Name))
            {
                var customerLike = new CustomerLike() {
                    CustomerId = customerId,
                    EntityId = entityId,
                    EntityName = typeof(T).Name,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
                };
                Repository.Insert(customerLike);
            }
            
        }

        public void Delete<T>(int customerId, int entityId)
        {
            var customerLike = GetCustomerLike<T>(customerId, entityId);
            if(customerLike != null)
                Repository.Delete(customerLike);
        }

        public int GetLikeCount<T>(int entityId)
        {
            return
                Repository.Table.Count(x => x.EntityId == entityId && x.EntityName == typeof(T).Name);
        }

        public IList<CustomerLike> GetEntityLikes<T>(int entityId)
        {
            return
                 Repository.Table.Where(x => x.EntityId == entityId && x.EntityName == typeof(T).Name)
                     .ToList();
        }
    }
}