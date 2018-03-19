using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Models;
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
            if (App.Authenticator != null)
            {
                authenticated = await App.Authenticator.Authenticate();
                if (authenticated)
                    App.Current.MainPage = new MainPage();
            }
        }
    }
}
