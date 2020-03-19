using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Models.User;
using Veganko.Services.Http;
using Xamarin.Essentials;

namespace Veganko.Services
{
    internal class AccountService : IAccountService
    {
        private readonly IRestService restService;

        public AccountService(IRestService restService)
        {
            this.restService = restService;
        }

        public UserPublicInfo User { get; set; }

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

            IRestResponse response = await restService.ExecuteAsync(request, false, throwIfUnsuccessful: false);
            if (response.IsSuccessful)
            {
                return JObject.Parse(response.Content).Value<string>("pwd_reset_token");
            }
            else
            {
                throw new ServiceException(response);
            }
        }

        public Task ResendConfirmationEmail(string email)
        {
            RestRequest request = new RestRequest("account/resend_confirmation_email", Method.GET);
            request.AddQueryParameter("email", email);
            return restService.ExecuteAsync(request, false);
        }
    }
}
