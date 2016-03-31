using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Extensions;
using Nop.Plugin.WebApi.MobSocial.Models;
using Nop.Plugin.WebApi.MobSocial.Services;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Media;

namespace Nop.Plugin.WebApi.MobSocial.Controllers
{
    [RoutePrefix("api/comments")]
    public class CommentApiController : BaseMobApiController
    {
        private readonly ICustomerCommentService _customerCommentService;
        private readonly ICustomerLikeService _customerLikeService;
        private readonly IWorkContext _workContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomerService _customerService;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;

        public CommentApiController(ICustomerCommentService customerCommentService, 
            IWorkContext workContext, 
            IDateTimeHelper dateTimeHelper, 
            ICustomerService customerService, 
            ICustomerLikeService customerLikeService, 
            IPictureService pictureService, MediaSettings mediaSettings)
        {
            _customerCommentService = customerCommentService;
            _workContext = workContext;
            _dateTimeHelper = dateTimeHelper;
            _customerService = customerService;
            _customerLikeService = customerLikeService;
            _pictureService = pictureService;
            _mediaSettings = mediaSettings;
        }

        [Route("post")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult Post(CustomerCommentModel model)
        {
            if (!ModelState.IsValid)
                return Response(new {Success = false});

            //save the comment
            var comment = new CustomerComment()
            {
                AdditionalData = model.AdditionalData,
                CommentText = model.CommentText,
                EntityName = model.EntityName,
                EntityId = model.EntityId,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                CustomerId = _workContext.CurrentCustomer.Id
            };
            _customerCommentService.Insert(comment);
            var cModel = PrepareCommentPublicModel(comment, new[] {_workContext.CurrentCustomer});
            return Response(new {Success = true});
        }

        [Route("get")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult Get([FromUri] CustomerCommentRequestModel model)
        {
            if (!ModelState.IsValid)
                return Response(new { Success = false });
            if (model.Page <= 0)
                model.Page = 1;
            if (model.Count <= 0)
                model.Count = 5;

            //retrieve the comments
            var comments = _customerCommentService.GetEntityComments(model.EntityId, model.EntityName, model.Page, model.Count);
            var commentModels = new List<CustomerCommentPublicModel>();

            //retrieve all the associated customers at ones for performance reasons
            var customers = _customerService.GetCustomersByIds(comments.Select(x => x.CustomerId).ToArray());

            foreach (var comment in comments)
            {
                var cModel = PrepareCommentPublicModel(comment, customers);
                commentModels.Add(cModel);
            }

            //send the response
            return Response(new {Success = true, Comments = commentModels});
        }

        [Route("delete/{commentId:int}")]
        [HttpDelete]
        [Authorize]
        public IHttpActionResult Delete(int commentId)
        {
            //only administrator or comment owner can delete the comment, so first let's retrieve the comment
            var comment = _customerCommentService.GetById(commentId);
            if(comment == null)
                return Response(new {Success = false, Message = "Comment doesn't exist"});
            //so who is ringing the bell?
            if(comment.CustomerId != _workContext.CurrentCustomer.Id && !_workContext.CurrentCustomer.IsAdmin())
                return Response(new { Success = false, Message = "Unauthorized" });

            //come in and delete the comment
            _customerCommentService.Delete(comment);

            return Response(new {Success = true});
        }


        #region helpers

        private CustomerCommentPublicModel PrepareCommentPublicModel(CustomerComment comment, IEnumerable<Customer> customers)
        {
            //get the customer
            var customer = customers.FirstOrDefault(x => x.Id == comment.CustomerId);
            if (customer == null)
                return null;
            //and create it's response model
            var cModel = new CustomerCommentPublicModel() {
                EntityName = comment.EntityName,
                EntityId = comment.EntityId,
                CommentText = comment.CommentText,
                AdditionalData = comment.AdditionalData,
                Id = comment.Id,
                DateCreatedUtc = comment.DateCreated,
                DateCreated = _dateTimeHelper.ConvertToUserTime(comment.DateCreated, DateTimeKind.Utc),
                CanDelete = comment.CustomerId == _workContext.CurrentCustomer.Id || _workContext.CurrentCustomer.IsAdmin(),
                IsSpam = false, //TODO: change it when spam system has been implemented
                LikeCount = _customerLikeService.GetLikeCount<CustomerComment>(comment.EntityId),
                CustomerName = customer.GetFullName(),
                CustomerProfileUrl = Url.RouteUrl("CustomerProfileUrl", new RouteValueDictionary()
                    {
                        {"SeName", customer.GetSeName(_workContext.WorkingLanguage.Id, true, false)}
                    }),
                CustomerProfileImageUrl = _pictureService.GetPictureUrl(
                    customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                    _mediaSettings.AvatarPictureSize, true)
            };
            return cModel;
        }
        #endregion
    }
}
