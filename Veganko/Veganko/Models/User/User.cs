using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Models.User
{
    public class User
    {
        public UserAccessRights AccessRights { get; set; }
        public string Id { get; set; }
        public string ProfileImage { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<int> Favorites { get; set; }
    }
}
