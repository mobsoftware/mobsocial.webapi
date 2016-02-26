using System.Collections.Generic;
using Mob.Core.Services;
using Nop.Core.Domain.Orders;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Enums;
using Nop.Plugin.WebApi.MobSocial.Models;
using Nop.Services.Payments;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface ISponsorPassService : IBaseEntityService<SponsorPass>
    {
        Order GetSponsorPassOrder(int SponsorPassOrderId);

        IList<Order> GetSponsorPassOrders(int SponsorCustomerId, int BattleId, BattleType BattleType);

        IList<SponsorPass> GetPurchasedSponsorPasses(int CustomerId, PassStatus? SponsorPassStatus);

        int CreateSponsorPass(BattleType BattleType, int BattleId, MobSocialProcessPaymentResultModel PaymentResponse, CustomerPaymentMethod PaymentMethod, decimal Amount);

        void MarkSponsorPassUsed(int SponsorPassOrderId, int BattleId, BattleType BattleType);

        SponsorPass GetSponsorPassByOrderId(int OrderId);


    }
}