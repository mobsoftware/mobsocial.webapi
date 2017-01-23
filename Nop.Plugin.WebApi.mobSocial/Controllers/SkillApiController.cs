using System.Linq;
using System.Web.Http;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.WebApi.MobSocial.Attributes;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Plugin.WebApi.MobSocial.Models;
using Nop.Plugin.WebApi.MobSocial.Services;
using Nop.Services.Customers;

namespace Nop.Plugin.WebApi.MobSocial.Controllers
{
    [RoutePrefix("api/skills")]
    public class SkillApiController : BaseMobApiController
    {
        #region fields
        private readonly ISkillService _skillService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        #endregion

        #region ctor
        public SkillApiController(ISkillService skillService, ICustomerService customerService, IWorkContext workContext)
        {
            _skillService = skillService;
            _customerService = customerService;
            _workContext = workContext;
        }
        #endregion

        #region actions
        [HttpGet]
        [Route("users/{userId:int}/get")]
        public IHttpActionResult GetUserSkills(int userId)
        {
            //check if the user exists or not
            var customer = _customerService.GetCustomerById(userId);
            if (customer == null)
                return NotFound();

            var userSkills = _skillService.GetUserSkills(userId).OrderBy(x => x.DisplayOrder);
            var model = userSkills.Select(x =>
            {
                var skillModel = new SkillModel() {
                    SkillName = x.SkillName,
                    CustomerId = x.CustomerId,
                    Description = x.Description,
                    DisplayOrder = x.DisplayOrder,
                    Id = x.Id
                };
                return skillModel;
            });

            return Response(new { Success = true, Skills = model });
        }

        [HttpGet]
        [Route("system/get")]
        public IHttpActionResult GetSystemSkills(int page = 1, int count = 15)
        {
            int total;
            var skills = _skillService.GetSystemSkills(out total, string.Empty, page, count);
            var model = skills.Select(x =>
            {
                var skillModel = new SkillModel() {
                    SkillName = x.SkillName,
                    Id = x.Id
                };
                return skillModel;
            });

            return Response(new { Success = true, Skills = model, Total = total });
        }

        [HttpGet]
        [ApiAuthorize]
        [Route("get/{id:int}")]
        public IHttpActionResult GetSkill(int id)
        {
            //get the skill first
            var skill = _skillService.GetById(id);
            if (skill == null)
                return NotFound();
            var model = new SkillModel() {
                DisplayOrder = skill.DisplayOrder,
                CustomerId = skill.CustomerId,
                SkillName = skill.SkillName,
                Description = skill.Description,
                Id = skill.Id
            };
            return Response(new { Success = true, Skill = model });
        }

        [HttpPost]
        [ApiAuthorize]
        [Route("post")]
        public IHttpActionResult Post(SkillModel model)
        {
            //if it's admin, we can safely change the customer id otherwise we'll save skill as logged in user 
            var isAdmin = _workContext.CurrentCustomer.IsAdmin();
            if (!isAdmin)
                model.CustomerId = _workContext.CurrentCustomer.Id;

            var skill = _skillService.GetById(model.Id) ?? new Skill();
            if (isAdmin)
            {
                if (model.CustomerId != 0) //if it's not a system skill
                {
                    //check if the customer exists
                    var skillCustomer = _customerService.GetCustomerById(model.CustomerId);
                    if (skillCustomer == null)
                    {
                        return Response(new {Success = false, Message = "Customer doesn't exist"});
                    }
                }
                else
                {
                    model.CustomerId = _workContext.CurrentCustomer.Id;
                }

            }
            else
            {
                if (model.CustomerId != _workContext.CurrentCustomer.Id)
                {
                    return Unauthorized();
                }
            }
            skill.CustomerId = model.CustomerId;
            skill.DisplayOrder = model.DisplayOrder;
            skill.SkillName = model.SkillName;
            skill.Description = model.Description;

            //insert/update the skill
            if (skill.Id == 0)
                _skillService.Insert(skill);
            else
                _skillService.Update(skill);

            return Response(new
            {
                Success = true,
                Skill = new SkillModel()
                {
                    SkillName = skill.SkillName,
                    Description = skill.Description,
                    Id = skill.Id,
                    CustomerId = skill.CustomerId,
                    DisplayOrder = skill.DisplayOrder
                }
            });
        }

        [HttpDelete]
        [ApiAuthorize]
        [Route("delete")]
        public IHttpActionResult Delete(int skillId)
        {
            var skill = _skillService.GetById(skillId);
            if (skill == null)
                return NotFound();

            if (skill.CustomerId != _workContext.CurrentCustomer.Id && !_workContext.CurrentCustomer.IsAdmin())
                return Unauthorized();

            //so we can safely delete this
            _skillService.Delete(skill);
            return Response(new { Success = true });
        } 
        #endregion
    }
}