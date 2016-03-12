using System.Web.Http;

namespace Nop.Plugin.WebApi.MobSocial.Controllers
{
    public class BaseMobApiController : ApiController
    {
        public IHttpActionResult Response(object obj)
        {
            //TODO: include xml response type if required
            return Json(obj);
        }
    }
}