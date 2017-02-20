namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface IMobSocialVideoProcessor
    {
        void WriteVideoThumbnailPicture(string videoFilePath, string imageFilePath);

        void WriteVideoThumbnailPicture(string videoFilePath, string imageFilePath, float? frameTime);

    }
}