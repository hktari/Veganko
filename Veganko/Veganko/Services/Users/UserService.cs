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
            RestRequest request = new RestRequest($"users/{user.Id}");
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

        public Task<IEnumerable<UserPublicInfo>> GetByIds(IEnumerable<string> id)
        {
            throw new NotImplementedException();
        }
    }
}
