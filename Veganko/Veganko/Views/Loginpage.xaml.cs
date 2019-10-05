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
        private async void OnLoginBtnClicked(object sender, EventArgs args)
        {
            if (string.IsNullOrWhiteSpace(vm.Email) || string.IsNullOrWhiteSpace(vm.Password))
            {
                await this.Err("Izpolnite vsa polja prosim :)");
                return;
            }

            try
            {
                vm.IsBusy = true;
                await vm.Login();
                NavigateToMainPage();
            }
            catch (ServiceException ex)
            {
                await this.Err($"{ex.StatusDescription}: {ex.Response}");
            }
            finally
            {
                vm.IsBusy = false;
            }
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