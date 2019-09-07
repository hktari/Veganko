using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VegankoService.Models.Auth
{
    public struct LoginResponse
    {
        public string Error { get; set; }
        public object Token { get; set; }
    }
}
