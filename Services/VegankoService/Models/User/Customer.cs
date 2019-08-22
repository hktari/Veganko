using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VegankoService.Models.User
{
    public class Customer
    {
        public string Id { get; set; }
        public string IdentityId { get; set; }
        public ApplicationUser Identity { get; set; }
        public int AccessRights { get; set; }
        public string ProfileBackgroundId { get; set; }
        public string AvatarId { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
    }
}
