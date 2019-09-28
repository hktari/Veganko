using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models.User;
using Veganko.Services.Auth;

namespace Veganko.Services.Http
{
    public class RestService : IRestService
    {
#if __ANDROID__
#if DEBUG
        private const string Endpoint = "https://10.0.2.2:5001/api";
#else
        private const string Endpoint = "https://77.38.119.234:5001/api";
#endif
#else
        private const string Endpoint = "https://localhost:5001/api";

#endif

        private readonly IRestClient client;
        
        public RestService()
        {
            client = new RestClient(Endpoint)
                .UseSerializer(() => new JsonNetSerializer());
            client.RemoteCertificateValidationCallback = (p1, p2, p3, p4) => true;
        }

        public IAuthService AuthService { get; set; }

        public async Task<TModel> ExecuteAsync<TModel>(RestRequest request, bool authorize = true)
            where TModel : new()
        {
            if (authorize)
            {
                await HandleAuthorization(request);
            }

            IRestResponse<TModel> response;
            try
            {
                response = await client.ExecuteTaskAsync<TModel>(request);
            }
            catch (Exception ex)
            {
                throw new ServiceException("Neznana napaka.", null, request.Resource, request.Method.ToString(), ex);
            }

            AssertResponseSuccess(response);
            return response.Data;
        }

        public async Task<IRestResponse> ExecuteAsync(RestRequest request, bool authorize = true, bool throwIfUnsuccessful = true)
        {
            if (authorize)
            {
                await HandleAuthorization(request);
            }
            
            IRestResponse response = await client.ExecuteTaskAsync(request);
            if (throwIfUnsuccessful)
            {
                AssertResponseSuccess(response);
            }

            return response;
        }

        private async Task HandleAuthorization(RestRequest request)
        {
            if (!await AuthService.IsTokenValid())
            {
                await AuthService.RefreshToken();
            }

            string token = await AuthService.GetToken();
            if (token == null)
            {
                throw new Exception("Failed to handle authorization. Token is null");
            }

            request.AddHeader("Authorization", "Bearer " + token);
        }

        private void AssertResponseSuccess(IRestResponse response)
        {
            if (!response.IsSuccessful)
            {
                string debugMsg = $"Http request failed: \n{response.Request.Resource}\n{response.Request.Method}\n{response.StatusCode}: {response.StatusDescription}";
                Console.WriteLine(debugMsg);
                throw new ServiceException(response.Content, response.StatusDescription, response.Request.Resource, response.Request.Method.ToString());
            }
        }

        public class JsonNetSerializer : IRestSerializer
        {
            public string Serialize(object obj) =>
                JsonConvert.SerializeObject(obj);

            public string Serialize(Parameter parameter) =>
                JsonConvert.SerializeObject(parameter.Value);

            public T Deserialize<T>(IRestResponse response) =>
                JsonConvert.DeserializeObject<T>(response.Content);

            public string[] SupportedContentTypes { get; } =
            {
                "application/json", "text/json", "text/x-json", "text/javascript", "*+json"
            };

            public string ContentType { get; set; } = "application/json";

            public DataFormat DataFormat { get; } = DataFormat.Json;
        }
    }
}
