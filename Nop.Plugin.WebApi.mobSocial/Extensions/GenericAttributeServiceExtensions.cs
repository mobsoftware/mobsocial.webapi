using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Common;

namespace Nop.Plugin.WebApi.MobSocial.Extensions
{
    public static  class GenericAttributeServiceExtensions
    {
        public static GenericAttribute GetAttributeByKey(this IGenericAttributeService genericAttributeService, BaseEntity entity, string key, int storeId = 0)
        {
            var entityName = entity.GetUnproxiedEntityType().Name;
            var ga = genericAttributeService.GetAttributesForEntity(entity.Id, entityName)
                .FirstOrDefault(
                    x =>
                        x.Key == key && 
                        x.StoreId == storeId);
            if (ga != null) //weird but it works this way only
                ga = genericAttributeService.GetAttributeById(ga.Id);
            return ga;
        }
    }
}
