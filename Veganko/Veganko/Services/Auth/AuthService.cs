using Newtonsoft.Json;
using RestSharp;
using System;
using System.Threading.Tasks;
using Veganko.Services.Http;
using Xamarin.Essentials;
using Veganko.Common.Models.Auth;
using Veganko.Services.Logging;

namespace Veganko.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IRestService restService;
        private readonly IUserService userService;
        private readonly ILogger logger;
        private DateTime? tokenExpiryDateUtc;
        private string token;
        private string email;
        private string password;

        public AuthService(IRestService restService, IUserService userService, ILogger logger)
        {
            this.restService = restService;
            this.userService = userService;
            this.logger = logger;
        }

        public async Task<IAuthService.LoginStatus> Login(string email, string password)
        {
            IAuthService.LoginStatus status = IAuthService.LoginStatus.Success;

            RestRequest loginRequest = new RestRequest("auth/login", Method.POST);
            loginRequest.AddJsonBody(
                new
                {
                    email,
                    password
                });

            var response = await restService.ExecuteAsync(loginRequest, authorize: false, throwIfUnsuccessful: false);

            if (response.ResponseStatus == ResponseStatus.Completed)
            {
                try
                {
                    LoginResponse responseData = JsonConvert.DeserializeObject<LoginResponse>(response.Content);

                    if (responseData.RequestError == null)
                    {
                        Token curToken = responseData.Token;
                        curToken.ExpiresAtUtc = DateTime.Now.AddSeconds(curToken.ExpiresIn);

                        await SetEmail(email);
                        await SetPassword(password);
                        await SetToken(curToken.AuthToken);
                        await SetTokenExpiryDateUtc(curToken.ExpiresAtUtc);

                        userService.SetCurrentUser(responseData.UserProfile);
                    }
                    else
                    {
                        status = ParseErrorCode((AuthErrorCode)responseData.RequestError.Status);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogException(new ServiceException(response, ex));
                    status = IAuthService.LoginStatus.UnknownError;
                }
            }
            else 
            {
                status = IAuthService.LoginStatus.Unreachable;
            }

            return status;
        }

        private IAuthService.LoginStatus ParseErrorCode(AuthErrorCode errorCode)
        {
            IAuthService.LoginStatus status;
            switch (errorCode)
            {
                case AuthErrorCode.InvalidCredentials:
                    status = IAuthService.LoginStatus.InvalidCredentials;
                    break;
                case AuthErrorCode.UnconfirmedEmail:
                    status = IAuthService.LoginStatus.UnconfirmedEmail;
                    break;
                case AuthErrorCode.Unknown:
                case AuthErrorCode.UserProfileNotFound:
                default:
                    status = IAuthService.LoginStatus.UnknownError;
                    break;
            }

            return status;
        }

        public void Logout()
        {
            SecureStorage.RemoveAll();
            tokenExpiryDateUtc = null;
            token = email = password = null;
        }

        public async Task<bool> CredentialsExist()
        {
            string email = await GetEmail();
            string password = await GetPassword();
            return email != null && password != null;
        }

        public async Task<bool> IsTokenValid()
        {
            tokenExpiryDateUtc = await GetTokenExpiryDateUtc();
            token = await GetToken();
            return token != null && tokenExpiryDateUtc != null && tokenExpiryDateUtc.Value > DateTime.Now;
        }

        public async Task RefreshToken()
        {
            string email = await GetEmail();
            string password = await GetPassword();
            if (email == null || password == null)
            {
                throw new Exception("Can't refresh token. Invalid credentials");
            }

            IAuthService.LoginStatus status = await Login(email, password);
            if (status != IAuthService.LoginStatus.Success)
            {
                logger.LogException(new Exception($"Failed to refresh token. Error code: {status}, email: {email}"));     
            }
        }

        #region Credentials Setters/Getters
        public async Task<string> GetToken()
        {
            if (token == null)
            {
                token = await SecureStorage.GetAsync("token");
            }

            return token;
        }

        private async Task SetToken(string token)
        {
            await SecureStorage.SetAsync("token", token);
            this.token = token;
        }

        private async Task<DateTime?> GetTokenExpiryDateUtc()
        {
            if (tokenExpiryDateUtc == null)
            {
                string tokenExpiryStr = await SecureStorage.GetAsync("tokenExpiryDateUtc");
                if (tokenExpiryStr != null)
                {
                    tokenExpiryDateUtc = DateTime.Parse(tokenExpiryStr);
                }
            }

            return tokenExpiryDateUtc;
        }

        private async Task SetTokenExpiryDateUtc(DateTime tokenExpiryDateUtc)
        {
            await SecureStorage.SetAsync("tokenExpiryDateUtc", tokenExpiryDateUtc.ToString());
            this.tokenExpiryDateUtc = tokenExpiryDateUtc;
        }

        private async Task<string> GetEmail()
        {
            if (email == null)
            {
                email = await SecureStorage.GetAsync("email");
            }

            return email;
        }

        private async Task SetEmail(string email)
        {
            await SecureStorage.SetAsync("email", email);
            this.email = email;
        }

        private async Task<string> GetPassword()
        {
            if (password == null)
            {
                password = await SecureStorage.GetAsync("password");
            }

            return password;
        }

        private async Task SetPassword(string password)
        {
            await SecureStorage.SetAsync("password", password);
            this.password = password;
        }
        #endregion

        //public async Task<bool> LoginWithFacebook()
        //{
        //    if (App.Authenticator == null)
        //        throw new Exception("Authentication dependency not found !");

        //    var authenticated = await App.Authenticator.Authenticate();
        //    if (!authenticated)
        //        return await Task.FromResult(false);

        //    var client = new HttpClient();
        //    client.DefaultRequestHeaders.Add("ACCESS_TOKEN", App.MobileService.CurrentUser.MobileServiceAuthenticationToken);
        //    var response = await client.GetAsync("https://vegankoauthenticationhelper.azurewebsites.net/api/GetUserDetails");
        //    var content = await response.Content.ReadAsStringAsync();
        //    var facebookData = JsonConvert.DeserializeObject<FacebookDetails>(content);
        //    User = new UserPublicInfo
        //    {
        //        Id = App.MobileService.CurrentUser?.UserId,
        //        Username = facebookData.FirstName,
        //        AvatarId = facebookData.Picture.Data.Url
        //    };
        //    return await Task.FromResult(true);
        //}
    }
}
