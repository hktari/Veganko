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
using Veganko.Extensions;
using Veganko.Services.Http;
using Veganko.Services.Logging;

namespace Veganko.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserService userService;
        private readonly ILogger logger;

        public bool IsManager { get; private set; }

        public LoginViewModel()
        {
            userService = App.IoC.Resolve<IUserService>();
            logger = App.IoC.Resolve<ILogger>();

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

        public async Task TryAutoLogin()
        {
            IsBusy = true;
            try
            {
                IAuthService authService = App.IoC.Resolve<IAuthService>();

                if (!await authService.CredentialsExist())
                {
                    return;
                }

                if (!await authService.IsTokenValid())
                {
                    await authService.RefreshToken();
                }

                SetupCurrentUser();
                NavigateToMainPage();
            }
            catch (Exception ex)
            {
                logger.LogException(
                    new Exception("Error while logging in automatically.", ex));
            }
            finally
            {
                IsBusy = false;
            }
        }

        public Command LoginCommand => new Command(async () =>
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await App.CurrentPage.Err("Izpolni vsa polja prosim :)");
                return;
            }

            try
            {
                IsBusy = true;
                IAuthService authService = App.IoC.Resolve<IAuthService>();

                await authService.Login(email, password);
                SetupCurrentUser();
                NavigateToMainPage();
            }
            catch (ServiceException ex)
            {
                logger.WriteLine<LoginViewModel>(ex.Message);
                await App.CurrentPage.Err($"Nepravilen email ali geslo.", ex);
            }
            finally
            {
                IsBusy = false;
            }
        });

        private void NavigateToMainPage()
        {
            App.Current.MainPage = new MainPage(IsManager);
        }

        private void SetupCurrentUser()
        {
            userService.EnsureCurrentUserIsSet();
            IsManager = userService.CurrentUser.IsManager();
        }
    }
}
