﻿using System;
using System.Web.Http;
using Mob.Core;
using Mob.Core.Domain;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.WebApi.MobSocial.Attributes;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Services;
using Nop.Web.Controllers;

namespace Nop.Plugin.WebApi.MobSocial.Controllers
{
    [RoutePrefix("api/customerfollow")]
    public class CustomerFollowApiController : BaseMobApiController
    {
        

        private readonly IWorkContext _workContext;
        private readonly ICustomerFollowService _customerFollowService;

        public CustomerFollowApiController(IWorkContext workContext, ICustomerFollowService customerFollowService)
        {
            _workContext = workContext;
            _customerFollowService = customerFollowService;
        }

        [HttpPost]
        [ApiAuthorize]
        [Route("follow/{entityName}/{id:int}")]
        public IHttpActionResult Follow(string entityName, int id)
        {
            var response = false;
            var newStatus = 0;
            switch (entityName.ToLower())
            {
                case FollowableEntityNames.VideoBattle:
                    response = Follow<VideoBattle>(id);
                    break;
                case FollowableEntityNames.Customer:
                    response = Follow<CustomerProfile>(id);
                    break;
                case FollowableEntityNames.Skill:
                    response = Follow<Skill>(id);
                    break;
            }
            if (response)
                newStatus = 1;
            return Json(new {Success = response, NewStatus = newStatus, NewStatusString = "Following"});
        }

        [HttpPost]
        [ApiAuthorize]
        [Route("unfollow/{entityName}/{id:int}")]
        public IHttpActionResult Unfollow(string entityName, int id)
        {
            var response = false;
            var newStatus = 1;
            switch (entityName.ToLower())
            {
                case FollowableEntityNames.VideoBattle:
                    response = Unfollow<VideoBattle>(id);
                    break;
                case FollowableEntityNames.Customer:
                    response = Unfollow<CustomerProfile>(id);
                    break;
                case FollowableEntityNames.Skill:
                    response = Unfollow<Skill>(id);
                    break;
            }
            if (response)
                newStatus = 0;
            return Json(new { Success = response, NewStatus = newStatus, NewStatusString = "Not Following" });
        }

        #region helpers
        private bool Follow<T>(int id) where T : IFollowSupported
        {

            _customerFollowService.Insert<T>(_workContext.CurrentCustomer.Id, id);
            return true;

        }

        private bool Unfollow<T>(int id) where T : IFollowSupported
        {
            _customerFollowService.Delete<T>(_workContext.CurrentCustomer.Id, id);
            return true;
        }

        #endregion

        #region inner classes

        private static class FollowableEntityNames
        {
            public const string VideoBattle = "videobattle";
            public const string Customer = "customer";
            public const string Skill = "skill";
        }

        #endregion
    }
}