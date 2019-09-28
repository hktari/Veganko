using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Models.User
{
    public class UserPublicInfo
    {
        public UserPublicInfo()
        {
        }

        public UserPublicInfo(UserPublicInfo user)
        {
            Update(user);
        }

        public void Update(UserPublicInfo user)
        {
            Id = user.Id;
            ProfileBackgroundId = user.ProfileBackgroundId;
            AvatarId = user.AvatarId;
            Username = user.Username;
            Email = user.Email;
            Description = user.Description;
            Label = user.Label;
            Role = user.Role;
        }

        public string Id { get; set; }

        public string ProfileBackgroundId { get; set; }

        public string AvatarId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Description { get; set; }

        public string Label { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public UserRole Role { get; set; }

        public bool IsManager()
        {
            return Role > UserRole.Member;
        }
    }
}
