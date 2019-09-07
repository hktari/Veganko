using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Veganko.Models;
using Veganko.Models.User;
using Veganko.Services;
using Veganko.Views;
using Xamarin.Forms;
using Autofac;
using System.Threading.Tasks;

namespace Veganko.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public string SelectedUserType { get; set; } = "user";

        public User User { get; set; }

        bool authenticated = false;

        public LoginViewModel()
        {
            User = new User();
        }

        private string password;
        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        private string email;
        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public async Task Login()
        {
            IAccountService accountService = App.IoC.Resolve<IAccountService>();

            if (SelectedUserType == "admin")
            {
                string username = null;
                string password = null;
                UserAccessRights uac;
                bool adminAccess = false;

                username = password = "admin";
                uac = UserAccessRights.All;
                adminAccess = true;
                await accountService.CreateAccount(new User { Username = username }, password);
                await accountService.Login(username, password);
                accountService.User.AccessRights = uac;
            }
            else
            {
                await accountService.Login(email, password);
            }
        }
    }
}
