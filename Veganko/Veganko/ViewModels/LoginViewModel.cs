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

namespace Veganko.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand => new Command(Login);
        public User User { get; set; }

        bool authenticated = false;

        public LoginViewModel()
        {
            User = new User();
        }

        private async void Login(object obj)
        {
            App.Current.MainPage = new MainPage();
            //var authenticated = await DependencyService.Get<IAccountService>().LoginWithFacebook();

            //if (authenticated)
            //{
            //    App.Current.MainPage = new MainPage();
            //}
        }
    }
}
