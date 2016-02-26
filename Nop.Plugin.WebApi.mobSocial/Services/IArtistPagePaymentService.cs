using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface IArtistPagePaymentService : IBaseEntityService<ArtistPagePayment>
    {
        void InsertPaymentMethod(ArtistPagePayment ArtistPagePayment);

        void DeletePaymentMethod(ArtistPagePayment ArtistPagePayment);

        void UpdatePaymentMethod(ArtistPagePayment ArtistPagePayment);

        ArtistPagePayment GetPaymentMethod(int ArtistPageId);
    }
}
