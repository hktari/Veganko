using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Extensions;
using Veganko.Services.Http;
using Veganko.ViewModels.PasswordRecovery;
using Veganko.Views.PasswordRecovery;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForgotPasswordPage : ContentPage
    {
        private ForgotPasswordViewModel vm;

        public ForgotPasswordPage()
        {
            InitializeComponent();
            BindingContext = this.vm = new ForgotPasswordViewModel();
        }

        private async void OnSendClicked(object sender, EventArgs args)
        {
            if (string.IsNullOrWhiteSpace(vm.Email))
            {
                await this.Err("Prosim vnesi email");
                return;
            }

            try
            {
                vm.IsBusy = true;
                await vm.SendForgotPasswordRequest();
                await Navigation.PushAsync(new ValidateOTPPage(vm));
            }
            catch (ServiceException ex)
            {
                await this.Err(ex.StatusDescription);
            }
            finally
            {
                vm.IsBusy = false;
            }
        }
    }
}