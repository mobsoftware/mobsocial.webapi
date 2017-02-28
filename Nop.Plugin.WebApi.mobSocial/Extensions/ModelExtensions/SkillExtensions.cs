using System.Linq;
using System.Web.Http.Routing;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Enums;
using Nop.Plugin.WebApi.MobSocial.Models;
using Nop.Plugin.WebApi.MobSocial.Services;
using Nop.Services.Customers;
using Nop.Services.Media;

namespace Nop.Plugin.WebApi.MobSocial.Extensions.ModelExtensions
{
    public static class SkillExtensions
    {
        public static SkillModel ToModel(this Skill skill)
        {
            var model = new SkillModel() {
                DisplayOrder = skill.DisplayOrder,
                SkillName = skill.Name,
                Id = skill.Id,
                UserId = skill.UserId
            };
            return model;
        }

        public static UserSkillModel ToModel(this UserSkill userSkill, IMediaService mediaService, MediaSettings mediaSettings, IWorkContext workContext, IStoreContext storeContext,
            ICustomerService customerService,
            ICustomerProfileViewService customerProfileViewService,
            ICustomerProfileService customerProfileService,
            IPictureService pictureService,
            UrlHelper url, bool onlySkillData = false, bool firstMediaOnly = false, bool withNextAndPreviousMedia = false, bool withSocialInfo = false)
        {
            var entityMedias = mediaService.GetEntityMedia<UserSkill>(userSkill.Id, null, count: int.MaxValue).ToList();
            var customer = onlySkillData ? null : customerService.GetCustomerById(userSkill.UserId);
            var model = new UserSkillModel() {
                DisplayOrder = userSkill.Skill.DisplayOrder,
                SkillName = userSkill.Skill.Name,
                UserSkillId = userSkill.Id,
                Id = userSkill.SkillId,
                User = onlySkillData ? null : customer.ToPublicModel(workContext, customerProfileViewService, customerProfileService, pictureService, mediaSettings, url),
                Media =
                    entityMedias.Take(firstMediaOnly ? 1 : 15)
                        .ToList()
                        .Select(
                            x =>
                                x.ToModel<UserSkill>(userSkill.Id, mediaService, mediaSettings, workContext, storeContext, customerService, customerProfileService, customerProfileViewService, pictureService, url))
                        .ToList(),
                TotalMediaCount = entityMedias.Count,
                TotalPictureCount = entityMedias.Count(x => x.MediaType == MediaType.Image),
                TotalVideoCount = entityMedias.Count(x => x.MediaType == MediaType.Video),
                ExternalUrl = userSkill.ExternalUrl,
                Description = userSkill.Description,
                SeName = userSkill.Skill.GetSeName(workContext.WorkingLanguage.Id)
            };
            return model;
        }

        public static SkillWithUsersModel ToSkillWithUsersModel(this Skill skill, IWorkContext workContext, IStoreContext storeContext, ICustomerService customerService, IUserSkillService userSkillService, IMediaService mediaService,
            MediaSettings mediaSettings, ICustomerFollowService followService, ICustomerLikeService likeService, ICustomerCommentService commentService, ICustomerProfileService customerProfileService, ICustomerProfileViewService customerProfileViewService, IPictureService pictureService, UrlHelper urlHelper)
        {
            var currentUser = workContext.CurrentCustomer;
            var model = new SkillWithUsersModel() {
                Skill = skill.ToModel(),
                FeaturedMediaImageUrl = skill.FeaturedImageId > 0 ? mediaService.GetPictureUrl(skill.FeaturedImageId) : ""
            };

            var perPage = 15;
            //by default we'll send data for 15 users. rest can be queried with paginated request
            //todo: make this thing configurable to set number of users to return with this response
            var userSkills = userSkillService.Get(x => x.SkillId == skill.Id).ToList();

            model.UserSkills =
                userSkills.Select(
                    x =>
                        x.ToModel(mediaService, mediaSettings, workContext, storeContext, customerService,
                            customerProfileViewService, customerProfileService, pictureService, urlHelper, true, true, true, false)).ToList();

            model.CurrentPage = 1;
            model.UsersPerPage = perPage;
            model.TotalUsers = userSkillService.Count(x => x.SkillId == skill.Id);
            model.FollowerCount = followService.GetFollowerCount<Skill>(skill.Id);

            //does this user follow this skill?
            var userFollow = followService.GetCustomerFollow<Skill>(currentUser.Id, skill.Id);
            model.CanFollow = currentUser.Id != skill.UserId;
            model.FollowStatus = userFollow == null ? 0 : 1;

            model.TotalComments = commentService.GetCommentsCount(skill.Id, "skill");
            model.LikeStatus = likeService.GetCustomerLike<Skill>(currentUser.Id, skill.Id) == null ? 0 : 1;
            model.TotalLikes = likeService.GetLikeCount<Skill>(skill.Id);
            return model;
        }
    }
}