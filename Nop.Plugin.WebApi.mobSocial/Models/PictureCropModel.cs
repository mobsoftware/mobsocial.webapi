using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class PictureCropModel : BaseNopModel
    {
        public int Left { get; set; }

        public int Top { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }
    }
}