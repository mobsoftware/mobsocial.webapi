using System.Collections.Generic;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class CustomerVideoAlbumModel : BaseNopModel
    {

        public CustomerVideoAlbumModel()
        {
            Videos = new List<CustomerVideoModel>();
        }
        public bool IsCurrentUsersProfile { get; set; }
        public int VideoAlbumId { get; set; } 
        public string Name { get; set; }

        public string CustomerFullName { get; set; }

        public List<CustomerVideoModel> Videos { get; set; }

        
    }
}


