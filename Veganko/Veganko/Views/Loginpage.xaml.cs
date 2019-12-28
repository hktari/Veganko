using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Veganko.Extensions;
using Veganko.Services.Http;
using Veganko.Views.PasswordRecovery;

namespace Veganko.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Loginpage : ContentPage
	{
        LoginViewModel vm;
		public Loginpage ()
		{
			InitializeComponent ();
            BindingContext = this.vm = new LoginViewModel();
		}

        protected async override void OnAppearing()
        {
            vm.IsBusy = true;
            try
            {
                if (await vm.TryAutoLogin())
                {
                    NavigateToMainPage();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while logging in automatically. " + ex.Message);
            }
            finally
            {
                vm.IsBusy = false;
            }
        }

        private async void OnSignUpBtnClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new RegisterPage());
        }

        private void NavigateToMainPage()
        {
            App.Current.MainPage = new MainPage(vm.IsManager);
        }

        private void OnForgotPasswordClicked(object sender, EventArgs args)
        {
            Navigation.PushAsync(new ForgotPasswordPage());
        }
    }
}