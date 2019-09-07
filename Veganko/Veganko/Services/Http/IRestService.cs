using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Veganko.Services.Http
{
    public interface IRestService
    {
        Task Login(string username, string password);

        Task<IRestResponse> ExecuteAsync(RestRequest request);

        Task<TModel> ExecuteAsync<TModel>(RestRequest request)
            where TModel : new();
    }
}
