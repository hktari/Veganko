using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Services.Http
{
    public class ServiceException : Exception
    {
        public ServiceException()
        {

        }

        public ServiceException(IRestResponse response)
            : this(response.ResponseStatus == ResponseStatus.Completed, response.Content, response.StatusDescription, response.Request.Resource, response.Request.Method.ToString(), response.ErrorException)
        {
            Errors = TryExtractErrors(response);
        }

        public ServiceException(string message, IRestResponse response)
            : this(response.ResponseStatus == ResponseStatus.Completed, message, response.StatusDescription, response.Request.Resource, response.Request.Method.ToString(), response.ErrorException)
        {
            Errors = TryExtractErrors(response);
        }

        public ServiceException(bool hasRemoteBeenReached, string response, string statusCodeDescription, string resource, string method)
            : this(hasRemoteBeenReached, response, statusCodeDescription, resource, method, null)
        {
            HasRemoteBeenReached = hasRemoteBeenReached;
            Response = response;
            StatusCodeDescription = statusCodeDescription;
            Resource = resource;
            Method = method;
        }

        public ServiceException(bool hasRemoteBeenReached, string response, string statusCodeDescription, string resource, string method, Exception innerException)
            : base($"HTTP request failed: {statusCodeDescription}", innerException)
        {
            HasRemoteBeenReached = hasRemoteBeenReached;
            Response = response;
            StatusCodeDescription = statusCodeDescription;
            Resource = resource;
            Method = method;
        }

        public bool HasRemoteBeenReached { get; }
        public string Response { get; }
        public string StatusCodeDescription { get; }
        public string Resource { get; }
        public string Method { get; }
        public IDictionary<string, string[]> Errors { get; set; }

        private IDictionary<string, string[]> TryExtractErrors(IRestResponse response)
        {
            try
            {
                var requestError = JsonConvert.DeserializeObject<RequestError>(response.Content);
                return requestError.Errors;
            }
            catch
            {
                return null;
            }
        }
    }
}
