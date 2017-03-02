using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Enums;
using Nop.Plugin.WebApi.MobSocial.Extensions.ModelExtensions;
using Nop.Plugin.WebApi.MobSocial.Models;
using Nop.Plugin.WebApi.MobSocial.Services;
using Nop.Services.Customers;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Plugin.WebApi.MobSocial.Controllers
{
    [RoutePrefix("api/skills")]
    public class SkillController : BaseMobApiController
    {
        #region fields
        private readonly ISkillService _skillService;
        private readonly ICustomerService _userService;
        private readonly IMediaService _mediaService;
        private readonly MediaSettings _mediaSettings;
        private readonly IUserSkillService _userSkillService;
        private readonly ICustomerFollowService _followService;
        private readonly ICustomerLikeService _likeService;
        private readonly ICustomerCommentService _commentService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerProfileService _customerProfileService;
        private readonly ICustomerProfileViewService _customerProfileViewService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IStoreContext _storeContext;
        #endregion

        #region ctor
        public SkillController(ISkillService skillService, ICustomerService userService, IMediaService mediaService, MediaSettings mediaSettings, IUserSkillService userSkillService, ICustomerFollowService followService, ICustomerLikeService likeService, ICustomerCommentService commentService, IWorkContext workContext, ICustomerProfileService customerProfileService, ICustomerProfileViewService customerProfileViewService, IPictureService pictureService, IUrlRecordService urlRecordService, IStoreContext storeContext)
        {
            _skillService = skillService;
            _userService = userService;
            _mediaService = mediaService;
            _mediaSettings = mediaSettings;
            _userSkillService = userSkillService;
            _followService = followService;
            _likeService = likeService;
            _commentService = commentService;
            _workContext = workContext;
            _customerProfileService = customerProfileService;
            _customerProfileViewService = customerProfileViewService;
            _pictureService = pictureService;
            _urlRecordService = urlRecordService;
            _storeContext = storeContext;
        }
        #endregion

        #region actions
        [HttpGet]
        [Route("users/{userId:int}/get")]
        public IHttpActionResult GetUserSkills(int userId)
        {
            //check if the user exists or not
            var customer = _userService.GetCustomerById(userId);
            if (customer == null)
                return NotFound();

            var userSkills = _skillService.GetUserSkills(userId).OrderBy(x => x.DisplayOrder);
            var model = userSkills.Select(x => x.ToModel(_mediaService, _mediaSettings, _workContext, _storeContext , _userService, _customerProfileViewService, _customerProfileService, _pictureService, Url));

            return Response(new { Success = true, Skills = model });
        }

        [HttpGet]
        [Route("get/all")]
        public IHttpActionResult GetSystemSkills(int page = 1, int count = 15)
        {
            int total;
            var skills = _skillService.GetAllSkills(out total, string.Empty, page, count);
            var model = skills.Select(x => x.ToModel(_workContext)).ToList();
            return Response(new { Success = true, Skills = model, Total = total });
        }

        [HttpGet]
        [Authorize]
        [Route("get/{id:int}")]
        public IHttpActionResult GetSkill(int id)
        {
            //get the skill first
            var skill = _skillService.GetById(id);
            if (skill == null)
                return NotFound();
            var model = skill.ToModel(_workContext);
            return Response(new { Success = true, Skill = model });
        }

        [HttpGet]
        [Authorize]
        [Route("get/{slug}")]
        public IHttpActionResult GetSkill(string slug)
        {
            var urlRecord = _urlRecordService.GetBySlug(slug);
            if (urlRecord == null || urlRecord.EntityName != typeof(Skill).Name)
                return NotFound();

            var skillId = urlRecord.EntityId;
            var skill = _skillService.GetById(skillId);

            if (skill == null)
                return NotFound();

            var model = skill.ToSkillWithUsersModel(_workContext, _storeContext,_userService, _userSkillService, _mediaService, _mediaSettings, _followService, _likeService, _commentService,_customerProfileService, _customerProfileViewService, _pictureService, Url);
            return Response(new { Success = true, SkillData = model });
        }

        [HttpGet]
        [Route("{id:int}/users/get")]
        public IHttpActionResult GetSkillUsers(int id, int page)
        {
            var skill = _skillService.GetById(id);
            if (skill == null)
                return NotFound();

            var userSkills = _userSkillService.Get(x => x.SkillId == id).ToList();
            var model =
                userSkills.Select(
                    x =>
                        x.ToModel(_mediaService, _mediaSettings, _workContext, _storeContext, _userService, _customerProfileViewService, _customerProfileService, _pictureService, Url, firstMediaOnly: true,
                            withNextAndPreviousMedia: true, withSocialInfo: false));

            return Response(new {
                Success = true,
                UserSkills = model
            });
        }

        [HttpPost]
        [Authorize]
        [Route("post")]
        public IHttpActionResult Post(UserSkillEntityModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var currentUser = _workContext.CurrentCustomer;
            //if it's admin, we can safely change the customer id otherwise we'll save skill as logged in user 
            var isAdmin = currentUser.IsAdmin();
            if (!isAdmin && model.UserId > 0)
                model.UserId = currentUser.Id;

            if (model.SystemSkill && isAdmin)
                model.UserId = 0;
            else
                model.UserId = currentUser.Id;

            var mediaIds = model.MediaId?.ToList() ?? new List<int>();
            //get all medias
            var medias = _mediaService.Get(x => mediaIds.Contains(x.Id) && x.UserId == currentUser.Id).ToList();

           
            //get skill, 1.) by id 2.) by name 3.) create new otherwise
            var skill = _skillService.GetById(model.Id) ??
                        (_skillService.FirstOrDefault(x => x.Name == model.SkillName) ?? new Skill() {
                            DisplayOrder = model.DisplayOrder,
                            UserId = currentUser.Id,
                            Name = model.SkillName
                        });

            //should we add this?
            if (skill.Id == 0)
            {
                _skillService.Insert(skill);
            }
            else
            {
                if (model.SystemSkill && isAdmin && skill.Name != model.SkillName)
                {
                    skill.Name = model.SkillName;
                    _skillService.Update(skill);
                }
            }

            //if user id is not 0, attach this skill with user
            if (model.UserId != 0)
            {
                var userSkill = model.UserSkillId > 0 ? _userSkillService.GetById(model.UserSkillId) : new UserSkill()
                {
                    UserId = model.UserId,
                    SkillId = skill.Id,
                    Description = model.Description,
                    DisplayOrder = model.DisplayOrder,
                    ExternalUrl = model.ExternalUrl
                };

                if (userSkill.Id == 0)
                    _userSkillService.Insert(userSkill);
                else
                    _userSkillService.Update(userSkill);

                //attach media if it exists
                foreach(var media in medias)
                    _mediaService.AttachMediaToEntity(userSkill, media);
                return Response(new {
                    Success = true,
                    Skill = userSkill.ToModel(_mediaService, _mediaSettings, _workContext, _storeContext, _userService, _customerProfileViewService, _customerProfileService, _pictureService, Url)
                });
            }
            return Response(new {
                Success = true,
                Skill = skill.ToModel(_workContext)
            });
        }

        [HttpPost]
        [Authorize]
        [Route("featured-media")]
        public IHttpActionResult Post(SetFeaturedMediaModel requestModel)
        {
            var skillId = requestModel.SkillId;
            var mediaId = requestModel.MediaId;
            var currentUser = _workContext.CurrentCustomer;
           //check if the skill and media actually exist?
            var skill = _skillService.GetById(skillId);
            if (skill == null)
                return NotFound();

            var canUpdate = currentUser.IsAdmin() || currentUser.Id == skill.UserId;

            if (!canUpdate)
                return Unauthorized();

            //check if media exist
            var media = _mediaService.GetById(mediaId);
            if (media == null || (media.UserId != currentUser.Id && !currentUser.IsAdmin()))
                return Unauthorized();

            //media should also be a picture to proceed further.
            //todo: support video covers as well
            if (media.MediaType != MediaType.Image)
                return BadRequest("Can't set media as featured image");

            skill.FeaturedImageId = mediaId;
            _skillService.Update(skill);
            return Response(new { Success = true});
        }

        [HttpDelete]
        [Authorize]
        [Route("user/media/delete/{userSkillId}/{mediaId}")]
        public IHttpActionResult DeleteMedia(int userSkillId, int mediaId)
        {
            var currentUser = _workContext.CurrentCustomer;
            //check if the skill and media actually exist?
            var userSkill = _userSkillService.GetById(userSkillId);
            if (userSkill == null)
                return NotFound();

            var canUpdate = currentUser.IsAdmin() || currentUser.Id == userSkill.UserId;

            if (!canUpdate)
                return Unauthorized();

            //check if media exist
            var media = _mediaService.GetById(mediaId);
            if (media == null || (media.UserId != currentUser.Id && !currentUser.IsAdmin()))
                return Unauthorized();

            _mediaService.DetachMediaFromEntity(userSkill, media);

            //delete the media as well
            _mediaService.Delete(media);
            return Response(new { Success = true});
        }

        [HttpDelete]
        [Authorize]
        [Route("delete/{skillId:int}")]
        public IHttpActionResult Delete(int skillId)
        {
           
            var currentUser = _workContext.CurrentCustomer;
            //current user must be admin to delete this skill
            if(!currentUser.IsAdmin())
                return Unauthorized();

            var skill = _skillService.GetById(skillId);
            if (skill == null)
                return NotFound();

            _userSkillService.Delete(x => x.SkillId == skillId);
            //so we can safely delete this
            _skillService.Delete(skill);
            return Response(new { Success = true});
        }

        [HttpDelete]
        [Authorize]
        [Route("users/delete/{userSkillId:int}")]
        public IHttpActionResult DeleteUserSkill(int userSkillId)
        {
            var currentUser = _workContext.CurrentCustomer;

            //first query user skill
            var userSkill = _userSkillService.GetById(userSkillId);

            //the current user should be either admin or himself to delete the skill
            if (userSkill.UserId != currentUser.Id && !currentUser.IsAdmin())
                return Unauthorized();

            //detach media
            _mediaService.ClearEntityMedia(userSkill);

            _userSkillService.Delete(userSkill);

            return Response(new { Success = true });
        }

        #endregion
    }
}