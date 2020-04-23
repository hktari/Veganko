﻿using Newtonsoft.Json;
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
using Veganko.Services.Logging;

namespace Veganko.Services.Http
{
    public class RestService : IRestService
    {
#if DEBUG
#if __ANDROID__
        //public const string Endpoint = "https://www.veganko.bid/api";
        public const string Endpoint = "http://10.0.2.2:5000/api";
#else
        public const string Endpoint = "https://www.veganko.bid/api";
        //public const string Endpoint = "http://localhost:5000/api";
#endif
#else
        public const string Endpoint = "https://www.veganko.bid/api";
#endif


        private readonly IRestClient client;
        private readonly ILogger logger;

        public RestService(ILogger logger)
        {
            client = new RestClient(Endpoint)
                .UseSerializer(() => new JsonNetSerializer(logger));
            client.Timeout = 10000; // 10s
            this.logger = logger;
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
                var sex = new ServiceException(false, "Neznana napaka.", null, request.Resource, request.Method.ToString(), ex);
                logger.LogException(sex);
                throw sex;
            }

            LogAnyTLErrors(response);
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

            LogAnyTLErrors(response);

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
                throw new ServiceException(response);
            }
        }

        /// <summary>
        /// Log transport layer errors
        /// </summary>
        /// <param name="response"></param>
        private void LogAnyTLErrors(IRestResponse response)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                logger.LogException(new ServiceException(response));
            }
        }

        public class JsonNetSerializer : IRestSerializer
        {
            private ILogger logger;

            public JsonNetSerializer(ILogger logger)
            {
                this.logger = logger;
            }

            public string Serialize(object obj) =>
                JsonConvert.SerializeObject(obj);

            public string Serialize(Parameter parameter) =>
                JsonConvert.SerializeObject(parameter.Value);

            public T Deserialize<T>(IRestResponse response)
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(response.Content);
                }
                catch (JsonSerializationException jse)
                {
                    logger.LogException(jse);
                    throw jse;
                }
            }
                
            public string[] SupportedContentTypes { get; } =
            {
                "application/json", "text/json", "text/x-json", "text/javascript", "*+json"
            };

            public string ContentType { get; set; } = "application/json";

            public DataFormat DataFormat { get; } = DataFormat.Json;
        }
    }
}
