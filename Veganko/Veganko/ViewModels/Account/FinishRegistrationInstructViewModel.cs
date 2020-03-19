using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Extensions;
using Veganko.Services;
using Veganko.Services.Http;
using Veganko.Services.Http.Errors;
using Xamarin.Forms;

namespace Veganko.ViewModels.Account
{
    public class FinishRegistrationInstructViewModel : BaseViewModel
    {
        public FinishRegistrationInstructViewModel(string email)
        {
            Email = email;
        }

        public Command ContinueCommand => new Command(
            async () => await App.Navigation.PopModalAsync());

        public Command ResendConfirmationEmail => new Command(
            async () =>
            {
                try
                {
                    IsBusy = true;
                    await AccountService.ResendConfirmationEmail(Email);
                    await App.CurrentPage.Inform("Potrditveni je bil email poslan.");
                }
                catch (ServiceException sex)
                {
                    await App.CurrentPage.Err("Pošiljanje ni uspelo.", sex);
                }
                finally
                {
                    IsBusy = false;
                }
            });

        public string Email { get; }

        private IAccountService AccountService => App.IoC.Resolve<IAccountService>();
    }
}
