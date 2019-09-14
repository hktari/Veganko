using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VegankoService.Models.Account
{
    public class ValidateOTPResponse
    {
        [JsonProperty("pwd_reset_token")]
        public string PwdResetToken { get; set; }
    }
}
