using System.Web.Http.Routing;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Models;
using Nop.Plugin.WebApi.MobSocial.Services;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Media;

namespace Nop.Plugin.WebApi.MobSocial.Extensions
{
    public static class CustomerExtensions
    {
        public static CustomerProfilePublicModel ToPublicModel(this Customer customer, 
            IWorkContext workContext, 
            ICustomerProfileViewService customerProfileViewService,
            ICustomerProfileService customerProfileService,
            IPictureService pictureService,
            MediaSettings mediaSettings,
            UrlHelper url)
        {
            
            var customerSeName = customer.GetSeName(workContext.WorkingLanguage.Id, true, false);
            var model = new CustomerProfilePublicModel()
            {
                CustomerId = customer.Id,
                ViewCount = customerProfileViewService.GetViewCount(customer.Id),
                FriendCount = customerProfileService.GetFriendCount(customer.Id),
                CustomerName = customer.GetFullName(),
                SeName = customerSeName,
                ProfileUrl = url.Route("CustomerProfileUrl", new {SeName = customerSeName}),
                ProfileImageUrl =
                    pictureService.GetPictureUrl(
                        customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                        mediaSettings.AvatarPictureSize, true),
                CoverImageUrl =
                    pictureService.GetPictureUrl(
                        customer.GetAttribute<int>(AdditionalCustomerAttributeNames.CoverImageId))
            };

            return model;
        }
    }
}
