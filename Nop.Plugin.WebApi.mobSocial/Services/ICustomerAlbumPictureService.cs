using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    /// <summary>
    /// Product service
    /// </summary>
    public interface ICustomerAlbumPictureService
    {

        byte[] CreateThumbnailPicture(byte[] pictureBinary, int maxWidth, string mimeType);
        CustomerAlbum GetCustomerAlbum(int customerId);
        CustomerAlbum GetCustomerAlbumById(int albumId);

        void Insert(CustomerAlbumPicture customerAlbumPicture);
        void Delete(CustomerAlbumPicture customerAlbumPicture);

        CustomerAlbumPicture GetCustomerAlbumPictureById(int customerAlbumPictureId);

        CustomerAlbum CreateCustomerMainAlbum(int customerId);


    }

}
