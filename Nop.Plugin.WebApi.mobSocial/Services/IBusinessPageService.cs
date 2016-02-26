using System.Collections.Generic;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    /// <summary>
    /// Product service
    /// </summary>
    public interface IBusinessPageService : IBaseEntityWithPictureService<BusinessPage, BusinessPagePicture>
    {
        List<BusinessPage> Search(string nameKeyword, string city, int? stateProvinceId, int? countryId);
    }

}
