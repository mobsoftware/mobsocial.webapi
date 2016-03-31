using System;
using System.Collections.Generic;
using System.Linq;
using Mob.Core.Data;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public class CustomerCommentService : BaseEntityService<CustomerComment>, ICustomerCommentService
    {
        public CustomerCommentService(IMobRepository<CustomerComment> repository)
            : base(repository)
        {
        }

        public override List<CustomerComment> GetAll(string Term, int Count = 15, int Page = 1)
        {
            throw new System.NotImplementedException();
        }

        public int GetCommentsCount(int entityId, string entityName)
        {
            return
                Repository.Table.Count(x => x.EntityId == entityId && x.EntityName == entityName);
        }

        public IList<CustomerComment> GetEntityComments(int entityId, string entityName, int page = 1, int count = 5)
        {
            return
                Repository.Table.Where(x => x.EntityId == entityId && x.EntityName == entityName)
                    .OrderBy(x => x.DateCreated)
                    .Skip(count*(page - 1))
                    .Take(count)
                    .ToList();
        }
    }
}
