﻿using Microsoft.WindowsAzure.MobileServices;
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
        public string SelectedUserType { get; set; }

        public Command LoginCommand => new Command(Login);
        public User User { get; set; }

        bool authenticated = false;

        public LoginViewModel()
        {
            User = new User();
        }

        private void Login(object obj)
        {
            IAccountService accountService = DependencyService.Get<IAccountService>();

            string username = null;
            string password = null;
            UserAccessRights uac;


            if (SelectedUserType == "admin")
            {
                username = password = "admin";
                uac = UserAccessRights.All;
            }
            else
            {
                username = password = "user";
                uac = UserAccessRights.ProductsRead | UserAccessRights.ProductsWrite;
            }

            accountService.CreateAccount(username, password, "avatar_fox.png");
            accountService.Login(username, password);
            accountService.User.AccessRights = uac;

            App.Current.MainPage = new MainPage();
            //var authenticated = await DependencyService.Get<IAccountService>().LoginWithFacebook();

            //if (authenticated)
            //{
            //    App.Current.MainPage = new MainPage();
            //}
        }
    }
}
