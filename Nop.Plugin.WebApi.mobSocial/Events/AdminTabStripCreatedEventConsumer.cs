using System;
using System.Web.Mvc;
using Nop.Admin.Controllers;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.WebApi.MobSocial.Constants;
using Nop.Plugin.WebApi.MobSocial.Controllers;
using Nop.Plugin.WebApi.MobSocial.Helpers;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Web.Framework.Events;

namespace Nop.Plugin.WebApi.MobSocial.Events
{
    public class AdminTabStripCreatedEventConsumer : IConsumer<AdminTabStripCreated>
    {
        private readonly ICustomerService _customerService;

        public AdminTabStripCreatedEventConsumer(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public void HandleEvent(AdminTabStripCreated eventMessage)
        {
            if (eventMessage.TabStripName != "customer-edit")
                return;

            //get the currently being edited customer id
            var evc = EngineContext.Current.Resolve<CustomerController>();
            var context = evc.ControllerContext ?? new ControllerContext(System.Web.HttpContext.Current.Request.RequestContext, evc);
            var customerId = Convert.ToInt32(context.RequestContext.RouteData.Values["id"]);
            var viewName = ViewHelpers.GetCorrectViewPath("Views/Customer/CustomerTab.cshtml");

            var content = ViewRenderer.RenderPartialView(viewName, customerId);
            eventMessage.BlocksToRender.Add(MvcHtmlString.Create(content));
        }
    }
}