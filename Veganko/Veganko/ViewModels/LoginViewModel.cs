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
using Veganko.Services.Auth;

namespace Veganko.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserService userService;

        public string SelectedUserType { get; set; } = "user";

        public bool IsManager { get; private set; }

        bool authenticated = false;

        public LoginViewModel()
        {
            userService = App.IoC.Resolve<IUserService>();

#if DEBUG
            Email = "bkamnik1995@gmail.com";
            Password = "test123";
#endif
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

        public async Task<bool> TryAutoLogin()
        {
            IAuthService authService = App.IoC.Resolve<IAuthService>();

            if (!await authService.CredentialsExist())
            {
                return false;
            }

            if (!await authService.IsTokenValid())
            {
                await authService.RefreshToken();
            }

            SetupCurrentUser();

            return true;
        }

        public async Task Login()
        {
            IAuthService authService = App.IoC.Resolve<IAuthService>();

            if (SelectedUserType == "admin")
            {
                // TODO: rework

                //string username = null;
                //string password = null;
                //UserAccessRights uac;
                //bool adminAccess = false;

                //username = password = "admin";
                //uac = UserAccessRights.All;
                //adminAccess = true;
                //await accountService.CreateAccount(new UserPublicInfo { Username = username }, password);
                //await accountService.Login(username, password);
                //accountService.User.AccessRights = uac;
            }
            else
            {
                await authService.Login(email, password);
                SetupCurrentUser();
            }
        }

        private void SetupCurrentUser()
        {
            userService.EnsureCurrentUserIsSet();
            IsManager = userService.CurrentUser.IsManager();
        }
    }
}
