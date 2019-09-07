using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Services.Http
{
    public class LoginResponse
    {
        public string Error { get; set; }
        public Token Token { get; set; }
    }
}
