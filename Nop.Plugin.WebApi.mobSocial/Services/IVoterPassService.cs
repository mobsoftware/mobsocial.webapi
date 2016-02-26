using System.Collections.Generic;
using Mob.Core.Services;
using Nop.Core.Domain.Orders;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Enums;
using Nop.Plugin.WebApi.MobSocial.Models;
using Nop.Services.Payments;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface IVoterPassService : IBaseEntityService<VoterPass>
    {
        Order GetVoterPassOrder(int VoterPassOrderId);

        IList<VoterPass> GetPurchasedVoterPasses(int CustomerId, PassStatus? VotePassStatus);

        int CreateVoterPass(BattleType BattleType, int BattleId, MobSocialProcessPaymentResultModel PaymentResponse, CustomerPaymentMethod PaymentMethod, decimal Amount);

        void MarkVoterPassUsed(int VoterPassOrderId);

        VoterPass GetVoterPassByOrderId(int OrderId);

        IList<Order> GetAllVoterPassOrders(BattleType BattleType, int BattleId, PassStatus? VoterPassStatus);
    }
}