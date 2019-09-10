using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models.User;
using Veganko.Services.Http;
using RestSharp.Serialization;
using Newtonsoft.Json;

namespace Veganko.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IRestService restService;

        public UserService(IRestService restService)
        {
            this.restService = restService;
        }

        public async Task<UserPublicInfo> Edit(UserPublicInfo user)
        {
            RestRequest request = new RestRequest($"users/{user.Id}", Method.PUT);
            request.AddJsonBody(user);

            IRestResponse response = await restService.ExecuteAsync(request, throwIfUnsuccessful: false);
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<UserPublicInfo>(response.Content);
            }
            else
            {
                throw new ServiceException(response);
            }
        }

        public Task<UserPublicInfo> Get(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserPublicInfo>> GetAll(int page = 1, int pageSize = 20)
        {
            RestRequest request = new RestRequest("users", Method.GET);
            request.AddQueryParameter(nameof(page), page.ToString());
            request.AddQueryParameter(nameof(pageSize), pageSize.ToString());

            IRestResponse response = await restService.ExecuteAsync(request, throwIfUnsuccessful: false);
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<IEnumerable<UserPublicInfo>>(response.Content);
            }
            else
            {
                throw new ServiceException(response);
            }
        }

        public Task<IEnumerable<UserPublicInfo>> GetByIds(IEnumerable<string> id)
        {
            throw new NotImplementedException();
        }
    }
}
