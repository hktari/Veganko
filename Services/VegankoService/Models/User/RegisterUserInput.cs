using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VegankoService.Models.User
{
    public class RegisterUserInput
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string ProfileBackgroundId { get; set; }
        public string AvatarId { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
    }
}
