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
using System.Diagnostics;

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
        private string msg;

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

                IAuthService.LoginStatus loginStatus = await authService.Login(email, password);
                if (loginStatus == IAuthService.LoginStatus.Success)
                {
                    SetupCurrentUser();
                    NavigateToMainPage();
                }
                else
                {
                    string errMsg = ParseLoginError(loginStatus);
                    if (loginStatus == IAuthService.LoginStatus.UnconfirmedEmail)
                    {
                        // TODO: navigate to page where you can resend confirmation email ? or resend on button press ? action sheet ?
                        await App.CurrentPage.Err(errMsg);
                    }
                    else
                    {
                        await App.CurrentPage.Err(errMsg);
                    }
                }

            }
            finally
            {
                IsBusy = false;
            }
        });

        private string ParseLoginError(IAuthService.LoginStatus err)
        {
            switch (err)
            {
                case IAuthService.LoginStatus.Unreachable:
                    msg = Veganko.Other.Strings.ServiceUnreachableErr;
                    break;
                case IAuthService.LoginStatus.InvalidCredentials:
                    msg = "Nepravilen email ali geslo.";
                    break;
                case IAuthService.LoginStatus.UnknownError:
                    msg = "Neznana napaka";
                    break;
                case IAuthService.LoginStatus.UnconfirmedEmail:
                    msg = "Email ni potrjen.";
                    break;
                default:
                    break;
            }

            return msg;
        }

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
