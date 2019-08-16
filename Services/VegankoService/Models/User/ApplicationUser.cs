using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VegankoService.Models.User
{
    public class ApplicationUser : IdentityUser
    {
        public int AccessRights { get; set; }
        public string ProfileBackgroundId { get; set; }
        public string AvatarId { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
        // public List<int> Favorites { get; set; }
    }
}
