using System.Linq;
using System.Web.Mvc;
using Nop.Plugin.WebApi.MobSocial.Helpers;
using Nop.Plugin.WebApi.MobSocial.Models;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Web.Controllers;

namespace Nop.Plugin.WebApi.MobSocial.Controllers
{
    public class MobSocialCustomerController : BasePublicController
    {
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;

        public MobSocialCustomerController(ICustomerService customerService, IGenericAttributeService genericAttributeService)
        {
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
        }

        [Authorize]
        public ActionResult CustomerTabEditor(int Id = 0)
        {
            if (Id == 0)
                return null;
            var visibleAttribute = _genericAttributeService.GetAttributesForEntity(Id, "Customer").FirstOrDefault(x => x.Key == "hideProfile");
            var model = new MobSocialCustomerModel
            {
                CustomerId = Id,
                HideProfile = visibleAttribute != null && visibleAttribute.Value == "True"
            };
            return View(ViewHelpers.GetCorrectViewPath("Views/Customer/CustomerTabContents.cshtml"), model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult CustomerTabEditor(int Id, MobSocialCustomerModel model)
        {
            //check if the customer exist
            var customer = _customerService.GetCustomerById(Id);
            if (customer == null)
                return Json(new {Success = false});

            //set the attributes
            _genericAttributeService.SaveAttribute(customer, "hideProfile", model.HideProfile);
            return Json(new { Success = true });
        }
    }
}