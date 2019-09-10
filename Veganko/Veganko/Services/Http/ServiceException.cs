using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Services.Http
{
    class ServiceException : Exception
    {
        public ServiceException(IRestResponse response)
            : this(response.Content, response.StatusDescription, response.Request.Resource, response.Request.Method.ToString(), response.ErrorException)
        {
        }

        public ServiceException(string response, string statusDescription, string resource, string method)
            : this(response, statusDescription, resource, method, null)
        {
            Response = response;
            StatusDescription = statusDescription;
            Resource = resource;
            Method = method;
        }

        public ServiceException(string response, string statusDescription, string resource, string method, Exception innerException)
            : base($"HTTP request failed: {statusDescription}", innerException)
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
