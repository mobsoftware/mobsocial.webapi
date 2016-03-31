using System.Collections.Generic;
using Mob.Core;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface ICustomerCommentService: IBaseEntityService<CustomerComment>
    {
        int GetCommentsCount(int entityId, string entityName);

        IList<CustomerComment> GetEntityComments(int entityId, string entityName, int page = 1, int count = 5);
    }
}