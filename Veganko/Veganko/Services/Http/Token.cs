using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Services.Http
{
    class Token
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("auth_token")]
        public string AuthToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonIgnore]
        public DateTime ExpiresAtUtc { get; set; }
    }
}
