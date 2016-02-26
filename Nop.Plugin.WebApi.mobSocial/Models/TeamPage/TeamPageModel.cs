using System.Collections.Generic;

namespace Nop.Plugin.WebApi.MobSocial.Models.TeamPage
{
    public class TeamPageModel
    {

        public TeamPageModel()
        {
            Groups = new List<TeamPageGroupModel>();
        }

        public List<TeamPageGroupModel> Groups { get; set; }

        public string TeamPictureUrl { get; set; }

        public string TeamName { get; set; }

        public string TeamDescription { get; set; }
    }
}