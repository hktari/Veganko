using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Models.User
{
    public class UserPublicInfo
    {
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
