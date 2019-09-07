using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Veganko.Services.Http
{
    public class RestService : IRestService
    {
        private const string Endpoint = "https://localhost:5001/api";
        private readonly IRestClient client;
        private Token curToken;
        private string username;
        private string password;

        public RestService()
        {
            client = new RestClient(Endpoint)
                .UseSerializer(() => new JsonNetSerializer());
            client.RemoteCertificateValidationCallback = (p1, p2, p3, p4) => true;
        }

        public async Task Login(string username, string password)
        {
            RestRequest loginRequest = new RestRequest("auth/login", Method.POST);
            loginRequest.AddJsonBody(
                new
                {
                    username,
                    password
                });

            curToken = await ExecuteAsync<Token>(loginRequest);
            curToken.ExpiresAtUtc = DateTime.UtcNow.AddSeconds(curToken.ExpiresIn);
            this.username = username;
            this.password = password;
        }

        public async Task<TModel> ExecuteAsync<TModel>(RestRequest request, bool authorize = true)
            where TModel : new()
        {
            if (authorize)
            {
                await HandleAuthorization(request);
            }

            IRestResponse<TModel> response = await client.ExecuteTaskAsync<TModel>(request);
            AssertResponseSuccess(response);

            return response.Data;
        }

        public async Task<IRestResponse> ExecuteAsync(RestRequest request, bool authorize = true)
        {
            if (authorize)
            {
                await HandleAuthorization(request);
            }

            IRestResponse response = await client.ExecuteTaskAsync(request);
            AssertResponseSuccess(response);
            return response;
        }

        private async Task HandleAuthorization(RestRequest request)
        {
            if (curToken == null)
            {
                throw new Exception("Login before using !");
            }
            else if (curToken.ExpiresAtUtc <= DateTime.UtcNow)
            {
                Debug.Assert(username != null);
                Debug.Assert(password != null);
                await Login(username, password);
            }

            request.AddHeader("Authorization", "Bearer " + curToken.AuthToken);
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
