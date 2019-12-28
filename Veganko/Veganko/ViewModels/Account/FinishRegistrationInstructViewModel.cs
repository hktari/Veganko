using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Veganko.ViewModels.Account
{
    public class FinishRegistrationInstructViewModel : BaseViewModel
    {
        public Command ContinueCommand => new Command(
            async () => await App.Navigation.PopModalAsync());
    }
}
