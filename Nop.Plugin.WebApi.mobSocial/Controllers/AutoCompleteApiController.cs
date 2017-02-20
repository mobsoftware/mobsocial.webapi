using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Http;
using Nop.Plugin.WebApi.MobSocial.Models;
using Nop.Plugin.WebApi.MobSocial.Services;

namespace Nop.Plugin.WebApi.MobSocial.Controllers
{
    [RoutePrefix("api/autocomplete")]
    public class AutoCompleteApiController : BaseMobApiController
    {
        private readonly ISkillService _skillService;

        public AutoCompleteApiController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        [Route("{autoCompleteType}/get")]
        [Authorize]
        public IHttpActionResult Get(string autoCompleteType, string searchText, int count = 10)
        {
            dynamic model = new ExpandoObject();
            if (autoCompleteType == "skills")
                model.Skills = GetSkills(searchText, count);

            return Response(new
            {
                Success = true,
                AutoComplete = model
            });
        }

        private IEnumerable<SkillModel> GetSkills(string searchText, int count = 10)
        {
            var skills = _skillService.SearchSkills(searchText, 1, count);
            var skillModels = skills.Select(x => new SkillModel() {
                SkillName = x.SkillName,
                DisplayOrder = x.DisplayOrder,
                Id = x.Id
            });
            return skillModels;
        }
    }
}