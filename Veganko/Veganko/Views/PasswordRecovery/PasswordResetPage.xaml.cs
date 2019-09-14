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
    public partial class PasswordResetPage : ContentPage
    {
        private readonly ForgotPasswordViewModel vm;

        public PasswordResetPage(ForgotPasswordViewModel vm)
        {
            InitializeComponent();
            BindingContext = this.vm = vm;
        }

        public async void OnResetPasswordClicked(object sender, EventArgs args)
        {
            if (string.IsNullOrWhiteSpace(vm.NewPassword))
            {
                await this.Err("Prosim izpolni polja.");
                return;
            }
            else if (vm.NewPassword != vm.ConfirmNewPassword)
            {
                await this.Err("Gesla se ne ujemata.");
                return;
            }

            try
            {
                vm.IsBusy = true;
                await vm.ResetPassword();
                await this.Inform("Geslo je spremenjeno.");
                await Navigation.PopToRootAsync();
            }
            catch (ServiceException ex)
            {
                await this.Err(ex.Response);
            }
            finally
            {
                vm.IsBusy = false;
            }
        }
    }
}