using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VegankoService.Models.User
{
    public class CustomerProfile
    {
        public CustomerProfile()
        { }

        public CustomerProfile(Customer input)
        {
            Id = input.Id;
            ProfileBackgroundId = input.ProfileBackgroundId;
            AvatarId = input.AvatarId;
            Description = input.Description;
            Label = input.Label;
        }

        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string ProfileBackgroundId { get; set; }
        public string AvatarId { get; set; }
        public string Role { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
    }
}
