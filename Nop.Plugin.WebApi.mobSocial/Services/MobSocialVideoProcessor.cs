using NReco.VideoConverter;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public class MobSocialVideoProcessor : IMobSocialVideoProcessor
    {
        public void WriteVideoThumbnailPicture(string videoFilePath, string imageFilePath)
        {
            WriteVideoThumbnailPicture(videoFilePath, imageFilePath, null);
        }      

        public void WriteVideoThumbnailPicture(string videoFilePath, string imageFilePath, float? frameTime)
        {
            var ffmpeg = new FFMpegConverter();
            ffmpeg.GetVideoThumbnail(videoFilePath, imageFilePath, frameTime);
        }
    }
}