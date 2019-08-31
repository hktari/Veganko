using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VegankoService.Models.User
{
    public class OTP
    {
        public string Id { get; set; }
        public int Code { get; set; }
        public string IdentityId { get; set; }
        public DateTime Timestamp { get; set; }
        public int LoginCount { get; set; }
    }
}
