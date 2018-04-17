using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;

[assembly: Xamarin.Forms.Dependency(typeof(Veganko.Services.AccountService))]
namespace Veganko.Services
{
    class AccountService : IAccountService
    {
        public User User { get; private set; }

        public bool CreateAccount(string username, string password, string profileImage)
        {
            throw new NotImplementedException();
        }

        public bool Login(string username, string password)
        {
            throw new NotImplementedException();
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
                ProfileImage = facebookData.Picture.Data.Url
            };
            return await Task.FromResult(true);
        }

        public bool Logout()
        {
            throw new NotImplementedException();
        }
    }
}
