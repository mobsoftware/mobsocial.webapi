using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;
using AutoMapper;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Extensions;
using Nop.Plugin.WebApi.MobSocial.Models;
using Nop.Plugin.WebApi.MobSocial.Models.TeamPage;
using Nop.Plugin.WebApi.MobSocial.Services;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Media;

namespace Nop.Plugin.WebApi.MobSocial.Controllers
{
    [RoutePrefix("api/teampage")]
    public class TeamPageApiController : BaseMobApiController
    {
        private readonly IWorkContext _workContext;
        private readonly ITeamPageService _teamPageService;
        private readonly ITeamPageGroupService _teamPageGroupService;
        private readonly ITeamPageGroupMemberService _teamPageGroupMemberService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerProfileViewService _customerProfileViewService;
        private readonly ICustomerProfileService _customerProfileService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;

        public TeamPageApiController(IWorkContext workContext, 
            ITeamPageService teamPageService, 
            ITeamPageGroupService teamPageGroupService, 
            ITeamPageGroupMemberService teamPageGroupMemberService, 
            ICustomerService customerService, 
            ICustomerProfileViewService customerProfileViewService, 
            IDateTimeHelper dateTimeHelper, 
            ICustomerProfileService customerProfileService, 
            IPictureService pictureService, 
            MediaSettings mediaSettings)
        {
            _workContext = workContext;
            _teamPageService = teamPageService;
            _teamPageGroupService = teamPageGroupService;
            _teamPageGroupMemberService = teamPageGroupMemberService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _customerProfileService = customerProfileService;
            _pictureService = pictureService;
            _mediaSettings = mediaSettings;
            _customerProfileViewService = customerProfileViewService;
        }

        [HttpPost]
        [Authorize]
        [Route("post")]
        public IHttpActionResult Post(TeamPageModel model)
        {
            if(!ModelState.IsValid || model == null)
                return Response(new { Success = false, Message = "Invalid data" });


            Mapper.CreateMap<TeamPageModel, TeamPage>();

            var teamPage = Mapper.Map<TeamPage>(model);
            teamPage.CreatedBy = _workContext.CurrentCustomer.Id;
            teamPage.CreatedOn = DateTime.UtcNow;
            teamPage.UpdatedOn = DateTime.UtcNow;
            //save to db now
            _teamPageService.Insert(teamPage);

            return Response(new
            {
                Success = true,
                Url = Url.RouteUrl("TeamPage", new RouteValueDictionary()
                {
                    {"TeamId", teamPage.Id}
                })
            });
        }

        [HttpPut]
        [Authorize]
        [Route("put")]
        public IHttpActionResult Put(TeamPageModel model)
        {
            if (!ModelState.IsValid || model == null || model.Id == 0)
                return Response(new { Success = false, Message = "Invalid data" });

            var teamPage = _teamPageService.GetById(model.Id);
            //check if the page exists or not & the person editing actually owns the resource
            if (teamPage.CreatedBy != _workContext.CurrentCustomer.Id && !_workContext.CurrentCustomer.IsAdmin())
            {
                return Response(new {
                    Success = false,
                    Message = "Unauthorized"
                });
            }

            Mapper.CreateMap<TeamPageModel, TeamPage>();
            Mapper.Map(model, teamPage);
          
            //update the updation date
            teamPage.UpdatedOn = DateTime.UtcNow;
            //update now
            _teamPageService.Update(teamPage);

            return Response(new { Success = true });
        }

        [HttpGet]
        [Route("get/{id:int}")]
        public IHttpActionResult Get(int id)
        {
            var teamPage = _teamPageService.GetById(id);
            if (teamPage == null)
            {
                return Response(new {
                    Success = false,
                    Message = "Team page not found"
                });
            }
            Mapper.CreateMap<TeamPage, TeamPageModel>();

            var model = Mapper.Map<TeamPageModel>(teamPage);

            return Response(new
            {
                Success = true,
                TeamPage = model
            });
        }

        [HttpGet]
        [Route("get/{seName}")]
        public IHttpActionResult Get(string seName)
        {
            var teamPage = _teamPageService.GetBySeName(seName);

            Mapper.CreateMap<TeamPage, TeamPageModel>();

            var model = Mapper.Map<TeamPageModel>(teamPage);

            return Response(new {
                Success = true,
                TeamPage = model
            });
        }

        [HttpDelete]
        [Route("delete/{id:int}")]
        [Authorize]
        public IHttpActionResult Delete(int id)
        {
            var teamPage = _teamPageService.GetById(id);
            if (teamPage == null)
            {
                return Response(new {
                    Success = false,
                    Message = "Team page not found"
                });
            }
            //check if the page exists or not & the person deleting actually owns the resource
            if (teamPage.CreatedBy != _workContext.CurrentCustomer.Id && !_workContext.CurrentCustomer.IsAdmin())
            {
                return Response(new {
                    Success = false,
                    Message = "Unauthorized"
                });
            }
            //delete the team page safely
            _teamPageService.SafeDelete(teamPage);

            return Response(new {
                Success = true
            });
        }

        [HttpPost]
        [Authorize]
        [Route("group/post")]
        public IHttpActionResult PostGroup(TeamPageGroupModel model)
        {
            if(!ModelState.IsValid || model == null || model.TeamId == 0)
                return Response(new { Success = false, Message = "Invalid data" });

            //check if the team page exists? and if it does, is the person creating the group has the authority
            var teamPage = _teamPageService.GetById(model.TeamId);
            if (teamPage == null)
            {
                return Response(new {
                    Success = false,
                    Message = "Team page not found"
                });
            }
            //check if the page exists or not & the person deleting actually owns the resource
            if (teamPage.CreatedBy != _workContext.CurrentCustomer.Id && !_workContext.CurrentCustomer.IsAdmin())
            {
                return Response(new {
                    Success = false,
                    Message = "Unauthorized"
                });
            }

            //ok, so we are good to save the group
            var group = new GroupPage()
            {
                TeamId = model.TeamId,
                Name = model.Name,
                Description = model.Description,
                PayPalDonateUrl = model.PayPalDonateUrl,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                DisplayOrder = model.DisplayOrder
            };

            _teamPageGroupService.Insert(group);
            return Response(new {
                Success = true
            });
        }
        [HttpPut]
        [Authorize]
        [Route("group/put")]
        public IHttpActionResult PutGroup(TeamPageGroupModel model)
        {
            if (!ModelState.IsValid || model == null || model.TeamId == 0 || model.Id == 0)
                return Response(new { Success = false, Message = "Invalid data" });

            //check if the team page exists? and if it does, is the person creating the group has the authority
            var teamPage = _teamPageService.GetById(model.TeamId);
            if (teamPage == null)
            {
                return Response(new {
                    Success = false,
                    Message = "Team page not found"
                });
            }
            //check if the page exists or not & the person deleting actually owns the resource
            if (teamPage.CreatedBy != _workContext.CurrentCustomer.Id && !_workContext.CurrentCustomer.IsAdmin())
            {
                return Response(new {
                    Success = false,
                    Message = "Unauthorized"
                });
            }
           //retrieve the group
            var groupPage = _teamPageGroupService.GetById(model.Id);
            if (groupPage == null)
            {
                return Response(new {
                    Success = false,
                    Message = "Group page not found"
                });
            }
            //create the model to entity mapping
            Mapper.CreateMap<TeamPageGroupModel, GroupPage>();

            //and map it
            Mapper.Map(model, groupPage);

            //update the group page now
            _teamPageGroupService.Update(groupPage);
            return Response(new {
                Success = true
            });
        }

        [Route("delete/{groupId:int}")]
        [HttpDelete]
        [Authorize]
        public IHttpActionResult DeleteGroup(int groupId)
        {
            //first retrieve the group and make sure that the right person is deleting the group
            var team = _teamPageService.GetTeamPageByGroup(groupId);
            if (team == null || team.CreatedBy != _workContext.CurrentCustomer.Id && !_workContext.CurrentCustomer.IsAdmin())
            {
                return Response(new {
                    Success = false,
                    Message = "Unauthorized"
                });
            }
            //get the group
            var group = team.GroupPages.FirstOrDefault(x => x.Id == groupId);
            if (group == null)
            {
                return Response(new {
                    Success = false,
                    Message = "Group page not found"
                });
            }
            //if there are more than one group and it's the default group that's being deleted
            if (group.IsDefault)
            {
                if (team.GroupPages.Count > 1)
                {
                    return Response(new
                    {
                        Success = false,
                        Message = "Can't delete the default group"
                    });
                }
                else
                {
                    //this is the last group, so safe delete
                    _teamPageGroupService.SafeDelete(group);
                }
            }
            else
            {
                //since it's not a default group, we'll need to move all members to default group
                //find the default group for this page
                var defaultGroup = team.GroupPages.FirstOrDefault(x => x.IsDefault);
                if (defaultGroup == null)
                {
                    //this should not hit unless direct changes have been made to database. we safe delete the group in this case
                    _teamPageGroupService.SafeDelete(group);
                }
                else
                {
                    //move all the members of this group to default group
                    foreach (var member in group.Members)
                    {
                        member.GroupPageId = defaultGroup.Id;
                        _teamPageGroupMemberService.Update(member);
                    }

                    //delete the group now
                    _teamPageGroupService.Delete(group);
                }
            }
            return Response(new {Success = true});

        }

        [Route("group/get/{teamId:int}")]
        [HttpGet]
        public IHttpActionResult GetTeamGroups(int teamId)
        {
            //first check if the team exists?
            var teamPage = _teamPageService.GetById(teamId);
            if (teamPage == null)
            {
                return Response(new {
                    Success = false,
                    Message = "Team page not found"
                });
            }
            //retrieve all group pages by team
            var groupPages = _teamPageGroupService.GetGroupPagesByTeamId(teamId);

            var listTeamPageGroups = new List<TeamPageGroupPublicModel>();

            //get all group members for performance
            var allMembers = _teamPageGroupMemberService.GetGroupPageMembersForTeam(teamId);
            var allMembersCustomerIds = allMembers.Select(x => x.CustomerId);

            var allCustomers = _customerService.GetCustomersByIds(allMembersCustomerIds.ToArray());
            foreach (var groupPage in groupPages)
            {
                var groupMembers = allMembers.Where(x => x.GroupPageId == groupPage.Id).OrderBy(x => x.DisplayOrder);
                //setup the individual group model
                var groupModel = new TeamPageGroupPublicModel()
                {
                    CreatedOnUtc = groupPage.DateCreated,
                    CreatedOn =  _dateTimeHelper.ConvertToUserTime(groupPage.DateCreated, DateTimeKind.Utc),
                    UpdatedOnUtc = groupPage.DateUpdated,
                    UpdatedOn = _dateTimeHelper.ConvertToUserTime(groupPage.DateUpdated, DateTimeKind.Utc),
                    Id = groupPage.Id,
                    PaypalDonateUrl = groupPage.PayPalDonateUrl,
                    TeamPageId = groupPage.TeamId,
                    Name = groupPage.Name,
                    Description = groupPage.Description,
                    GroupMembers = new List<CustomerProfilePublicModel>()
                };
                //add customers' public models to the list
                foreach (var member in groupMembers)
                {
                    var memberCustomer = allCustomers.First(x => x.Id == member.CustomerId);
                    groupModel.GroupMembers.Add(memberCustomer.ToPublicModel(_workContext, _customerProfileViewService,
                        _customerProfileService, _pictureService, _mediaSettings, Url));

                }

                //add this group model to the output list
                listTeamPageGroups.Add(groupModel);
            }

            //send the response
            return Json(new
            {
                Success = true,
                TeamGroups = listTeamPageGroups
            });
        }

    }
}