using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Services.Http.Errors
{
    public class RequestValidationError : RequestError
    {
        public IDictionary<string, string[]> Errors { get; set; }
    }
}
