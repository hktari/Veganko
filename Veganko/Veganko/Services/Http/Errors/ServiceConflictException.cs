using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Services.Http.Errors
{
    public class ServiceConflictException<TConflictingItem> : Exception
    {
        public ServiceConflictException(RequestConflictError<TConflictingItem> requestConflict)
        {
            RequestConflict = requestConflict;
        }

        public RequestConflictError<TConflictingItem> RequestConflict { get; set; }
    }
}
