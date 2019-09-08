using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models.User;

namespace Veganko.Services.Http
{
    public interface IRestService
    {
        Task<UserPublicInfo> Login(string email, string password);

        Task<IRestResponse> ExecuteAsync(RestRequest request, bool authorize = true);

        Task<TModel> ExecuteAsync<TModel>(RestRequest request, bool authorize = true)
            where TModel : new();
    }
}
