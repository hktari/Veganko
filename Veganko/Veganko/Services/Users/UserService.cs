using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models.User;
using Veganko.Services.Http;
using RestSharp.Serialization;
using Newtonsoft.Json;
using Veganko.Common.Models.Users;
using Veganko.Models;
using Veganko.Services.Storage;

namespace Veganko.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IRestService restService;
        private readonly IPreferences preferences;
        private UserPublicInfo currentUser;

        public UserService(IRestService restService, IPreferences preferences)
        {
            this.restService = restService;
            this.preferences = preferences;
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
                    preferences.Set("currentUser", response.Content);
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
            RestRequest request = new RestRequest($"users/{id}", Method.GET);
            return restService.ExecuteAsync<UserPublicInfo>(request);
        }

        public async Task<PagedList<UserPublicInfo>> GetAll(UserQuery query)
        {
            RestRequest request = new RestRequest("users", Method.GET);
            request.AddQueryParameter(nameof(UserQuery.Role), query.Role.ToString());
            request.AddQueryParameter(nameof(UserQuery.Page), query.Page.ToString());
            request.AddQueryParameter(nameof(UserQuery.PageSize), query.PageSize.ToString());
            request.AddQueryParameter(nameof(UserQuery.Name), query.Name);

            IRestResponse response = await restService.ExecuteAsync(request, throwIfUnsuccessful: false);
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<PagedList<UserPublicInfo>>(response.Content);
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

        public Task SetRole(UserPublicInfo user, UserRole role)
        {
            RestRequest request = new RestRequest($"userroles", Method.PUT);
            request.AddQueryParameter("userId", user.Id);
            request.AddJsonBody(new { role = role.ToString() });
            return restService.ExecuteAsync(request);
        }

        public async Task EnsureCurrentUserIsSet()
        {
            if (currentUser == null)
            {
                string serializedCurUser = preferences.Get("currentUser", null);
                if (serializedCurUser != null)
                {
                    try
                    {
                        currentUser = JsonConvert.DeserializeObject<UserPublicInfo>(serializedCurUser);
                        currentUser = await Get(currentUser.Id); // Reload the data, since User.Role might've changed.
                        CoerceInvalidProperties(currentUser);
                    }
                    catch (JsonSerializationException ex)
                    {
                        throw new Exception($"Failed to deserialize {nameof(CurrentUser)} from cache.", ex);
                    }
                }
                else
                {
                    throw new Exception($"Failed to load current user from cache. This means a valid token exists, but user data doesn't.");
                }
            }
        }

        public void SetCurrentUser(UserPublicInfo user)
        {
            CoerceInvalidProperties(user);
            currentUser = user;
            preferences.Set("currentUser", JsonConvert.SerializeObject(user));
        }

        public void ClearCurrentUser()
        {
            preferences.Remove("currentUser");
            currentUser = null;
        }

        private void CoerceInvalidProperties(UserPublicInfo user)
        {
            user.AvatarId = string.IsNullOrEmpty(user.AvatarId) ? "0" : user.AvatarId;
            user.ProfileBackgroundId = string.IsNullOrEmpty(user.ProfileBackgroundId) ? "0" : user.ProfileBackgroundId;
        }
    }
}
