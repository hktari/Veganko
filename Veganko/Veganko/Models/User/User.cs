using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Models.User
{
    public class User
    {
        public UserAccessRights AccessRights { get; set; }
        public string Id { get; set; }
        public string ProfileBackgroundId { get; set; }
        public string AvatarId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
        public List<int> Favorites { get; set; }

        public bool CanApproveProducts()
        {
            const UserAccessRights mask = UserAccessRights.ProductsDelete;
            return (AccessRights & mask) == mask;
        }
    }
}
