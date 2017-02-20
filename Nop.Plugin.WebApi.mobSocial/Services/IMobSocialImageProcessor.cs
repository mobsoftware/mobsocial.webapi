using System.Drawing.Imaging;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface IMobSocialImageProcessor
    {
        void WriteBytesToImage(byte[] imageBytes, string filePath, ImageFormat imageFormat);

        byte[] ResizeImage(byte[] imageBytes, int width, int height);

        void WriteResizedImage(byte[] imageBytes, int width, int height, string filePath, ImageFormat imageFormat);

        void WriteResizedImage(string sourceFile, string destinationFile, int width, int height, ImageFormat imageFormat);
    }
}