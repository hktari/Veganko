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

namespace Veganko.Services
{
    class AccountService : IAccountService
    {
        private readonly IRestService restService;

        public AccountService(IRestService restService)
        {
            this.restService = restService;
        }

        public UserPublicInfo User { get; private set; }

        public Task CreateAccount(UserPublicInfo user, string password)
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

            return restService.ExecuteAsync(request, false);
        }

        public async Task Login(string email, string password)
        {
            User = await restService.Login(email, password);
        }

        public Task ForgotPassword(string email)
        {
            RestRequest request = new RestRequest("account/forgotpassword", Method.POST);
            request.AddJsonBody(new { email });
            return restService.ExecuteAsync(request, false);
        }

        public Task ResetPassword(string email, string token, string newPassword)
        {
            RestRequest request = new RestRequest("account/resetpassword", Method.POST);
            request.AddJsonBody(
                new
                {
                    email,
                    token,
                    password = newPassword,
                });

            return restService.ExecuteAsync(request, false);
        }

        public async Task<string> ValidateOTP(string email, int otp)
        {
            RestRequest request = new RestRequest("account/otp", Method.POST);
            request.AddJsonBody(
                new
                {
                    otp,
                    email
                });

            IRestResponse response = await restService.ExecuteAsync(request, false);
            return response.Content;
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
            User = new UserPublicInfo
            {
                Id = App.MobileService.CurrentUser?.UserId,
                Username = facebookData.FirstName,
                AvatarId = facebookData.Picture.Data.Url
            };
            return await Task.FromResult(true);
        }

        public void Logout()
        {
            User = null;
            restService.Logout();
        }
    }
}
