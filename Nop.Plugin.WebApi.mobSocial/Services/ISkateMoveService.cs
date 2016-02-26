using System.Collections.Generic;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    /// <summary>
    /// Product service
    /// </summary>
    public interface ISkateMoveService
    {
        List<SkateMove> GetAll();

        List<CustomerSkateMove> GetCustomerSkateMoves(int customerId);

    }

}
