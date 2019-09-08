using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Models.User;

namespace VegankoService.Models.Auth
{
    public struct LoginResponse
    {
        public string Error { get; set; }
        public object Token { get; set; }
        public CustomerProfile Profile { get; set; }
    }
}
