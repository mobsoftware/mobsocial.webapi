using System.Collections.Generic;
using Mob.Core.Data;
using Mob.Core.Services;
using Nop.Core;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Services.Seo;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public class EntityMediaService : BaseEntityService<EntityMedia>, IEntityMediaService
    {
        public EntityMediaService(IMobRepository<EntityMedia> repository) : base(repository) {}
        public EntityMediaService(IMobRepository<EntityMedia> repository, IWorkContext workContext, IUrlRecordService urlRecordService) : base(repository, workContext, urlRecordService) {}
        public override List<EntityMedia> GetAll(string Term, int Count = 15, int Page = 1)
        {
            throw new System.NotImplementedException();
        }
    }
}