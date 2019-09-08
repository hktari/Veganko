using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Models.User;

namespace Veganko.Services.Http
{
    public class LoginResponse
    {
        public string Error { get; set; }

        public Token Token { get; set; }

        [JsonProperty("profile")]
        public UserPublicInfo UserProfile { get; set; }
    }
}
