using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Services.Http.Errors
{
    public class RequestConflictError<TConflictingItem> : RequestError
    {
        public TConflictingItem ConflictingItem { get; set; }

        public string[] ConflictingFields { get; set; }
    }
}
