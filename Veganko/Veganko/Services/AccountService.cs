using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Models.User;
using Veganko.Services.Http;

//[assembly: Xamarin.Forms.Dependency(typeof(Veganko.Services.AccountService))]
namespace Veganko.Services
{
    class AccountService : IAccountService
    {
        private readonly IRestService restService;

        public AccountService(IRestService restService)
        {
            this.restService = restService;
        }

        public User User { get; private set; }

        public Task CreateAccount(User user, string password)
        {
            RestRequest request = new RestRequest("account", Method.POST);
            request.AddJsonBody(
                new
                {
                    userName = user.Username,
                    email = user.Email,
                    password,
                    description = user.Description,
                    label = user.Label,
                    avatarId = user.AvatarId,
                    profileBackgroundId = user.ProfileBackgroundId
                });

            return restService.ExecuteAsync(request);
        }

        public Task Login(string username, string password)
        {
            return restService.Login(username, password);
        }

        public async Task<bool> LoginWithFacebook()
        {
            if (App.Authenticator == null)
                throw new Exception("Authentication dependency not found !");

            var authenticated = await App.Authenticator.Authenticate();
            if (!authenticated)
                return await Task.FromResult(false);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("ACCESS_TOKEN", App.MobileService.CurrentUser.MobileServiceAuthenticationToken);
            var response = await client.GetAsync("https://vegankoauthenticationhelper.azurewebsites.net/api/GetUserDetails");
            var content = await response.Content.ReadAsStringAsync();
            var facebookData = JsonConvert.DeserializeObject<FacebookDetails>(content);
            User = new User
            {
                Id = App.MobileService.CurrentUser?.UserId,
                Username = facebookData.FirstName,
                AvatarId = facebookData.Picture.Data.Url
            };
            return await Task.FromResult(true);
        }

        public bool Logout()
        {
            throw new NotImplementedException();
        }
    }
}
