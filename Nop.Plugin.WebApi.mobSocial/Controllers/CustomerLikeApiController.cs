using System.Web.Http;
using Nop.Core;
using Nop.Plugin.WebApi.MobSocial.Attributes;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Services;

namespace Nop.Plugin.WebApi.MobSocial.Controllers
{
    [RoutePrefix("api/customerlike")]
    public class CustomerLikeApiController : BaseMobApiController
    {
        

        private readonly IWorkContext _workContext;
        private readonly ICustomerLikeService _customerLikeService;

        public CustomerLikeApiController(IWorkContext workContext, ICustomerLikeService customerLikeService)
        {
            _workContext = workContext;
            _customerLikeService = customerLikeService;
        }

        [HttpPost]
        [ApiAuthorize]
        [Route("like/{entityName}/{id:int}")]
        public IHttpActionResult Like(string entityName, int id)
        {
            var response = false;
            var newStatus = 0;
            switch (entityName.ToLower())
            {
                case LikableEntityNames.VideoBattle:
                    response = Like<VideoBattle>(id);
                    break;
                case LikableEntityNames.Customer:
                    response = Like<CustomerProfile>(id);
                    break;
                case LikableEntityNames.TimelinePost:
                    response = Like<TimelinePost>(id);
                    break;
                case LikableEntityNames.CustomerComment:
                    response = Like<CustomerComment>(id);
                    break;
            }
            if (response)
                newStatus = 1;
            return Json(new {Success = response, NewStatus = newStatus, NewStatusString = "Liked"});
        }

        [HttpPost]
        [ApiAuthorize]
        [Route("unlike/{entityName}/{id:int}")]
        public IHttpActionResult Unlike(string entityName, int id)
        {
            var response = false;
            var newStatus = 1;
            switch (entityName.ToLower())
            {
                case LikableEntityNames.VideoBattle:
                    response = Unlike<VideoBattle>(id);
                    break;
                case LikableEntityNames.Customer:
                    response = Unlike<CustomerProfile>(id);
                    break;
                case LikableEntityNames.TimelinePost:
                    response = Unlike<TimelinePost>(id);
                    break;
                case LikableEntityNames.CustomerComment:
                    response = Unlike<CustomerComment>(id);
                    break;
            }
            if (response)
                newStatus = 0;
            return Json(new { Success = response, NewStatus = newStatus, NewStatusString = "Unliked" });
        }

        #region helpers
        private bool Like<T>(int id)
        {

            _customerLikeService.Insert<T>(_workContext.CurrentCustomer.Id, id);
            return true;

        }

        private bool Unlike<T>(int id)
        {
            _customerLikeService.Delete<T>(_workContext.CurrentCustomer.Id, id);
            return true;
        }

        #endregion

        #region inner classes

        private static class LikableEntityNames
        {
            public const string VideoBattle = "videobattle";
            public const string Customer = "customer";
            public const string TimelinePost = "timelinepost";
            public const string CustomerComment = "customercomment";
        }

        #endregion
    }
}