using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Extensions;
using Veganko.Services.Http;
using Veganko.ViewModels.PasswordRecovery;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views.PasswordRecovery
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ValidateOTPPage : ContentPage
    {
        private readonly ForgotPasswordViewModel vm;

        public ValidateOTPPage(ForgotPasswordViewModel vm)
        {
            InitializeComponent();
            BindingContext = this.vm = vm;
        }

        private async void OnValidateClicked(object sender, EventArgs args)
        {
            if (string.IsNullOrWhiteSpace(vm.OTP))
            {
                await this.Err("Prosim izpolni polje.");
                return;
            }

            try
            {
                vm.IsBusy = true;
                await vm.ValidateOTP();
                await Navigation.PushAsync(new PasswordResetPage(vm));
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