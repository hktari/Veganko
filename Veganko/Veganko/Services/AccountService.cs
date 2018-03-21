using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Models;

[assembly: Xamarin.Forms.Dependency(typeof(Veganko.Services.AccountService))]
namespace Veganko.Services
{
    class AccountService : IAccountService
    {
        public User User
        {
            get
            {
                return new User
                {
                    Username = App.MobileService.CurrentUser != null ? App.MobileService.CurrentUser.UserId : "",
                    ProfileImage = "icon.png"
                };
            }
        }

        public bool CreateAccount(string username, string password, string profileImage)
        {
            throw new NotImplementedException();
        }

        public bool Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public bool Logout()
        {
            throw new NotImplementedException();
        }
    }
}
