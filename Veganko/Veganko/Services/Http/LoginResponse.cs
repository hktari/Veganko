using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Common.Models.Users;
using Veganko.Models.User;
using Veganko.Services.Http.Errors;

namespace Veganko.Services.Http
{
    public class LoginResponse
    {
        public RequestError RequestError { get; set; }

        public Token Token { get; set; }

        [JsonProperty("profile")]
        public UserPublicInfo UserProfile { get; set; }
    }
}
