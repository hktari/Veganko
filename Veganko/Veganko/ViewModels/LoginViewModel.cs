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

namespace Veganko.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public string SelectedUserType { get; set; } = "user";

        public Command LoginCommand => new Command(Login);
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

        private async void Login(object obj)
        {
            IAccountService accountService = App.IoC.Resolve<IAccountService>();

            string username = null;
            string password = null;
            UserAccessRights uac;
            bool adminAccess = false;

            if (SelectedUserType == "admin")
            {
                username = password = "admin";
                uac = UserAccessRights.All;
                adminAccess = true;
            }
            else
            {
                username = password = "user";
                uac = UserAccessRights.ProductsRead | UserAccessRights.ProductsWrite;
            }

            accountService.CreateAccount(new User { Username = username }, password);
            accountService.Login(username, password);
            accountService.User.AccessRights = uac;

            App.Current.MainPage = new MainPage(adminAccess);
            //var authenticated = await DependencyService.Get<IAccountService>().LoginWithFacebook();

            //if (authenticated)
            //{
            //    App.Current.MainPage = new MainPage();
            //}
        }
    }
}
