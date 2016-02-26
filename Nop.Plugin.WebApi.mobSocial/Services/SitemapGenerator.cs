using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Security;
using Nop.Plugin.WebApi.MobSocial.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Topics;
using SeoExtensions = Nop.Plugin.WebApi.MobSocial.Extensions.SeoExtensions;
using Nop.Services.Seo;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    /// <summary>
    /// Represents a sitemap generator
    /// </summary>
    public partial class SitemapGenerator : Nop.Services.Seo.SitemapGenerator
    {

        private ICustomerService _customerService;
        private readonly IBusinessPageService _businessPageService;
        private IEventPageService _eventPageService;
        private IWorkContext _workContext;

        public SitemapGenerator(IStoreContext storeContext, 
            ICategoryService categoryService, 
            IProductService productService, 
            IManufacturerService manufacturerService, 
            ITopicService topicService, 
            CommonSettings commonSettings, 
            IEventPageService eventPageService, 
            ICustomerService customerService,
            IBusinessPageService businessPageService,
            BlogSettings blogSettings, 
            NewsSettings newsSettings, 
            ForumSettings forumSettings,
            SecuritySettings securitySettings, IWorkContext workContext) : 
                base(storeContext,
                categoryService,
                productService,
                manufacturerService,
                topicService,
                commonSettings,
                blogSettings,
                newsSettings,
                forumSettings,
                securitySettings)
        {
            _customerService = customerService;
            _businessPageService = businessPageService;
            _workContext = workContext;
            _eventPageService = eventPageService;
        }


        /// <summary>
        /// Method that is overridden, that handles creation of child urls.
        /// Use the method WriteUrlLocation() within this method.
        /// </summary>
        /// <param name="urlHelper">URL helper</param>
        protected override void GenerateUrlNodes(UrlHelper urlHelper)
        {
            base.GenerateUrlNodes(urlHelper);

            //if (_mobSocialSettings.SitemapIncludeEvents)
            //{
            WriteEventPages(urlHelper);
            WriteProfilePages(urlHelper);
            WriteBusinessPages(urlHelper);
            //}

        }


        private void WriteEventPages(UrlHelper urlHelper)
        {
            var eventPages = _eventPageService.GetAllUpcomingEvents();

            foreach (var eventPage in eventPages)
            {
                var url = urlHelper.RouteUrl("EventPageUrl", new { SeName = SeoExtensions.GetSeName(eventPage, _workContext.WorkingLanguage.Id) }, "http");
                var updateFrequency = UpdateFrequency.Weekly;
                var updateTime = eventPage.DateUpdated;
                WriteUrlLocation(url, updateFrequency, updateTime);
            }

        }



        private void WriteProfilePages(UrlHelper urlHelper)
        {
            var customers = _customerService.GetAllCustomers();

            var registeredCustomers = customers.Where(x => !x.Deleted && x.Active);

            foreach (var customer in registeredCustomers)
            {
                var url = urlHelper.RouteUrl("CustomerProfileUrl", new { SeName = SeoExtensions.GetSeName(customer, _workContext.WorkingLanguage.Id) }, "http");

                if (url != null)
                {
                    var updateTime = customer.LastActivityDateUtc;
                    WriteUrlLocation(url, UpdateFrequency.Weekly, updateTime);
                }

            }

        }

        private void WriteBusinessPages(UrlHelper urlHelper)
        {
            var businessPages = _businessPageService.GetAll();

            foreach (var businessPage in businessPages)
            {
                var url = urlHelper.RouteUrl("BusinessPageUrl", new { SeName = SeoExtensions.GetSeName(businessPage, _workContext.WorkingLanguage.Id) }, "http");

                if (url != null)
                {
                    var updateTime = businessPage.DateUpdated;
                    WriteUrlLocation(url, UpdateFrequency.Weekly, updateTime);
                }
            }

        }
    }
}
