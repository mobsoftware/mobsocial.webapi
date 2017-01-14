using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using AutoMapper;
using Mob.Core;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Plugin.WebApi.MobSocial.Attributes;
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
        private readonly mobSocialSettings _mobSocialSettings;

        public TeamPageApiController(IWorkContext workContext, 
            ITeamPageService teamPageService, 
            ITeamPageGroupService teamPageGroupService, 
            ITeamPageGroupMemberService teamPageGroupMemberService, 
            ICustomerService customerService, 
            ICustomerProfileViewService customerProfileViewService, 
            IDateTimeHelper dateTimeHelper, 
            ICustomerProfileService customerProfileService, 
            IPictureService pictureService, 
            MediaSettings mediaSettings, 
            mobSocialSettings mobSocialSettings)
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
            _mobSocialSettings = mobSocialSettings;
            _customerProfileViewService = customerProfileViewService;

            Mapper.Initialize(cfg => cfg.CreateMap<TeamPageModel, ConfigurationModel>());
            Mapper.Initialize(cfg => cfg.CreateMap<TeamPageModel, TeamPage>());
            Mapper.Initialize(cfg => cfg.CreateMap<TeamPageGroupModel, GroupPage>());
        }

        [HttpPost]
        [ApiAuthorize]
        [Route("post")]
        public IHttpActionResult Post(TeamPageModel model)
        {
            if(!ModelState.IsValid || model == null)
                return Response(new { Success = false, Message = "Invalid data" });

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
        [ApiAuthorize]
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

            Mapper.Map(model, teamPage);
          
            //update the updation date
            teamPage.UpdatedOn = DateTime.UtcNow;
            //update now
            _teamPageService.Update(teamPage);

            return Response(new { Success = true });
        }

        [HttpPut]
        [ApiAuthorize]
        [Route("cover/put/{id:int}/{pictureId:int}")]
        public IHttpActionResult UpdateTeamPicture(int id, int pictureId)
        {
            //first retrieve the team page
            var teamPage = _teamPageService.GetById(id);
            //is the current user authorized to perform this operation
            if (teamPage == null ||
                (teamPage.CreatedBy != _workContext.CurrentCustomer.Id && !_workContext.CurrentCustomer.IsAdmin()))
            {
                return Json(new
                {
                    Success = false,
                    Message = "Unauthorized"
                });
            }

            teamPage.TeamPictureId = pictureId;

            //save now
            _teamPageService.Update(teamPage);

            return Json(new
            {
                Success = true
            });

        }

        [HttpGet]
        [Route("get/{id:int}")]
        public IHttpActionResult Get(int id)
        {
            var teamPage = _teamPageService.GetById(id);
            if (teamPage == null)
            {
                return NotFound();
            }
            var model = new TeamPageModel()
            {
                CreatedBy = teamPage.CreatedBy,
                CreatedOn = teamPage.CreatedOn,
                Description = teamPage.Description,
                Id = teamPage.Id,
                Name = teamPage.Name,
                TeamPictureId = teamPage.TeamPictureId,
                UpdatedBy = teamPage.UpdatedBy,
                UpdatedOn = teamPage.UpdatedOn
            };

            model.Groups = GetTeamPageGroupPublicModels(id);
            //is the page editable
            model.IsEditable = _workContext.CurrentCustomer.IsAdmin() ||
                               _workContext.CurrentCustomer.Id == teamPage.CreatedBy;

            model.TeamPictureUrl = _pictureService.GetPictureUrl(model.TeamPictureId, 0, false);
            return Response(new
            {
                Success = true,
                TeamPage = model
            });
        }

        [HttpGet]
        [Route("get/my")]
        [ApiAuthorize]
        public IHttpActionResult Get()
        {
            var teamPages = _teamPageService.GetTeamPagesByOwner(_workContext.CurrentCustomer.Id);

            var model = new List<TeamPageModel>();
            foreach (var page in teamPages)
            {
                var pModel = Mapper.Map<TeamPageModel>(page);
                pModel.PageUrl = Url.RouteUrl("TeamPage", new RouteValueDictionary()
                {
                    { "teamId", page.Id }
                });
                model.Add(pModel);
            }

            return Response(new {
                Success = true,
                TeamPages = model
            });
        }

        [HttpGet]
        [Route("get/{seName}")]
        public IHttpActionResult Get(string seName)
        {
            var teamPage = _teamPageService.GetBySeName(seName);

            var model = Mapper.Map<TeamPageModel>(teamPage);

            return Response(new {
                Success = true,
                TeamPage = model
            });
        }

        [HttpDelete]
        [Route("delete/{id:int}")]
        [ApiAuthorize]
        public IHttpActionResult Delete(int id)
        {
            var teamPage = _teamPageService.GetById(id);
            if (teamPage == null)
            {
                return NotFound();
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
        [ApiAuthorize]
        [Route("group/post")]
        public IHttpActionResult PostGroup(TeamPageGroupModel model)
        {
            if(!ModelState.IsValid || model == null || model.TeamPageId == 0)
                return Response(new { Success = false, Message = "Invalid data" });

            //check if the team page exists? and if it does, is the person creating the group has the authority
            var teamPage = _teamPageService.GetById(model.TeamPageId);
            if (teamPage == null)
            {
                return NotFound();
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
                TeamId = model.TeamPageId,
                Name = model.Name,
                Description = model.Description,
                PayPalDonateUrl = model.PayPalDonateUrl,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                DisplayOrder = model.DisplayOrder
            };

            _teamPageGroupService.Insert(group);
            return Response(new {
                Success = true,
                Id = group.Id
            });
        }
        [HttpPut]
        [ApiAuthorize]
        [Route("group/put")]
        public IHttpActionResult PutGroup(TeamPageGroupModel model)
        {
            if (!ModelState.IsValid || model == null || model.TeamPageId == 0 || model.Id == 0)
                return Response(new { Success = false, Message = "Invalid data" });

            //check if the team page exists? and if it does, is the person creating the group has the authority
            var teamPage = _teamPageService.GetById(model.TeamPageId);
            if (teamPage == null)
            {
                return NotFound();
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
                return NotFound();
            }

            //and map it
            Mapper.Map(model, groupPage);

            //if the current group is default group, the other one should be set as non-default
            if (model.IsDefault)
            {
                //first get all groups of this team
                var groupPages = _teamPageGroupService.GetGroupPagesByTeamId(model.TeamPageId);
                foreach (var gp in groupPages)
                {
                    if (gp.Id != groupPage.Id)
                    {
                        //set default false and update the group
                        gp.IsDefault = false;
                        _teamPageGroupService.Update(gp);
                    }
                }
            }

            //update the current group page now
            _teamPageGroupService.Update(groupPage);
            return Response(new {
                Success = true,
                Id = groupPage.Id
            });
        }

        [Route("group/delete/{groupId:int}")]
        [HttpDelete]
        [ApiAuthorize]
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
                return NotFound();
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
            //safe delete the group. replacement for the code written below
            _teamPageGroupService.SafeDelete(group);

            //the code below was commented to avoid any confusion regarding deletion
            //todo: decide whether we should keep the code below
            /*
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
            }*/
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
                return NotFound();
            }

            var listTeamPageGroups = GetTeamPageGroupPublicModels(teamId);

            //send the response
            return Json(new
            {
                Success = true,
                TeamGroups = listTeamPageGroups
            });
        }

        [ApiAuthorize]
        [HttpPost]
        [Route("group/members/post")]
        public IHttpActionResult PostGroupMembers(TeamPageGroupMemberModel model)
        {
            //we'll have to check each group and then each member in question to see if they both exist and that a 
            //group member entry already exist for the combination
            //lets first query all the members
            var members = _customerService.GetCustomersByIds(model.CustomerId);
            if(members.Count == 0)
                return Json(new
                {
                    Success = false,
                    Message = "Members don't exist"
                });

            //does this team exist
            var team = _teamPageService.GetById(model.TeamId);
            if (team == null ||
                (team.CreatedBy != _workContext.CurrentCustomer.Id && !_workContext.CurrentCustomer.IsAdmin()))
            {
                return Json(new {
                    Success = false,
                    Message = "Unauthorized"
                });
            }

            
            //if user hasn't passed any group id, it's better to add default group for processing
            if (model.GroupId.Length == 0)
            {
                var defaultGroup = team.GroupPages.FirstOrDefault(x => x.IsDefault);
                if (defaultGroup == null)
                {
                    defaultGroup = team.GroupPages.FirstOrDefault();
                    if (defaultGroup == null)
                    {
                        return Json(new {
                            Success = false,
                            Message = "No groups found in the team page"
                        });
                    }
                }
                model.GroupId = new[] {defaultGroup.Id};
            }
            
            foreach (var groupId in model.GroupId)
            {
                //check if the group exist
                var group = team.GroupPages.FirstOrDefault(x => x.Id == groupId);
                if (group == null)
                {
                    continue; //skip as group doesn't exist
                }

                //let's find existing group members
                var groupMembers = group.Members;

                //if group validation succeed, we loop through the members and add those members which don't exist
                foreach (var member in members)
                {
                    if (groupMembers.All(x => x.CustomerId != member.Id))
                    {
                        //let's add this combination
                        _teamPageGroupMemberService.Insert(new GroupPageMember()
                        {
                            CustomerId = member.Id,
                            GroupPageId = group.Id,
                            DateCreated = DateTime.UtcNow,
                            DateUpdated = DateTime.UtcNow,
                            DisplayOrder = 0
                        });
                    }
                }
            }
            return Json(new
            {
                Success = true
            });
        }

        [Route("group/members/delete/{groupId}/{memberId}")]
        [HttpDelete]
        [ApiAuthorize]
        public IHttpActionResult DeleteGroupMember(int groupId, int memberId)
        {
            //first check if the group exist?
            var group = _teamPageGroupService.GetById(groupId);
            if (group == null)
            {
                return NotFound();
            }

            //check if the user adding is authorized to do that
            if (group.Team.CreatedBy != _workContext.CurrentCustomer.Id && !_workContext.CurrentCustomer.IsAdmin())
            {
                return Json(new {
                    Success = false,
                    Message = "Unauthorized"
                });
            }
            //delete the page member
            _teamPageGroupMemberService.DeleteGroupPageMember(groupId, memberId);

            return Json(new
            {
                Success = true
            });

        }

        [ApiAuthorize]
        [Route("cover/post")]
        [HttpPost]
        public IHttpActionResult UploadCover()
        {
            var files = HttpContext.Current.Request.Files;
            if (files.Count == 0)
                return Response(new { Success = false, Message = "No file uploaded" });

            //the file
            var file = files[0];

            //and it's name
            var fileName = file.FileName;
            //stream to read the bytes
            var stream = file.InputStream;
            var pictureBytes = new byte[stream.Length];
            stream.Read(pictureBytes, 0, pictureBytes.Length);

            //file extension and it's type
            var fileExtension = Path.GetExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            var contentType = file.ContentType;

            if (string.IsNullOrEmpty(contentType))
            {
                contentType = PictureUtility.GetContentType(fileExtension);
            }
            //save the picture now
            var picture = _pictureService.InsertPicture(pictureBytes, contentType, null);
            var image = new
            {
                ImageUrl = _pictureService.GetPictureUrl(picture.Id),
                SmallImageUrl = _pictureService.GetPictureUrl(picture.Id, _mobSocialSettings.TimelineSmallImageWidth),
                ImageId = picture.Id,
                MimeType = contentType
            };

            return Json(new { Success = true, Image = image });
        }

        #region helpers

        private List<TeamPageGroupPublicModel> GetTeamPageGroupPublicModels(int teamId)
        {
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
                var groupModel = new TeamPageGroupPublicModel() {
                    CreatedOnUtc = groupPage.DateCreated,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(groupPage.DateCreated, DateTimeKind.Utc),
                    UpdatedOnUtc = groupPage.DateUpdated,
                    UpdatedOn = _dateTimeHelper.ConvertToUserTime(groupPage.DateUpdated, DateTimeKind.Utc),
                    Id = groupPage.Id,
                    DisplayOrder = groupPage.DisplayOrder,
                    IsDefault =  groupPage.IsDefault,
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
            return listTeamPageGroups;
        }
        #endregion

    }
}