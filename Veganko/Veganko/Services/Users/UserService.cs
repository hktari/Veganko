using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models.User;
using Veganko.Services.Http;
using RestSharp.Serialization;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Veganko.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IRestService restService;
        private UserPublicInfo currentUser;

        public UserService(IRestService restService)
        {
            this.restService = restService;
        }

        public UserPublicInfo CurrentUser
        {
            get
            {
                return currentUser 
                    ?? throw new Exception($"{nameof(CurrentUser)} hasn't been set. Make sure '{nameof(EnsureCurrentUserIsSet)}' has been called.");
            }
            private set
            {
                currentUser = value;
            }
        }

        public async Task<UserPublicInfo> Edit(UserPublicInfo user)
        {
            RestRequest request = new RestRequest($"users/{user.Id}", Method.PUT);
            request.AddJsonBody(user);

            IRestResponse response = await restService.ExecuteAsync(request, throwIfUnsuccessful: false);
            if (response.IsSuccessful)
            {
                UserPublicInfo updatedUser = JsonConvert.DeserializeObject<UserPublicInfo>(response.Content);

                // Update the CurrentUser reference and cache if the user being edited is the current user.
                if (currentUser?.Id == updatedUser.Id)
                {
                    Preferences.Set("currentUser", response.Content);
                    currentUser.Update(updatedUser);
                }

                return updatedUser;
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

        public void EnsureCurrentUserIsSet()
        {
            if (currentUser == null)
            {
                string serializedCurUser = Preferences.Get("currentUser", null);
                if (serializedCurUser != null)
                {
                    try
                    {
                        currentUser = JsonConvert.DeserializeObject<UserPublicInfo>(serializedCurUser);
                    }
                    catch (JsonSerializationException ex)
                    {
                        throw new Exception($"Failed to deserialize {nameof(CurrentUser)} from cache.", ex);
                    }
                }
            }
        }

        public void SetCurrentUser(UserPublicInfo user)
        {
            currentUser = user;
            Preferences.Set("currentUser", JsonConvert.SerializeObject(user));
        }

        public void ClearCurrentUser()
        {
            Preferences.Remove("currentUser");
            currentUser = null;
        }
    }
}
