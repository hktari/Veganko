using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Services.Http
{
    class ServiceException : Exception
    {
        public ServiceException(string response, string statusDescription, string resource, string method)
            : this(response, statusDescription, resource, method, null)
        {
            Response = response;
            StatusDescription = statusDescription;
            Resource = resource;
            Method = method;
        }

        public ServiceException(string response, string statusDescription, string resource, string method, Exception innerException)
        {
            Response = response;
            StatusDescription = statusDescription;
            Resource = resource;
            Method = method;
        }

        public string Response { get; }
        public string StatusDescription { get; }
        public string Resource { get; }
        public string Method { get; }
    }
}
