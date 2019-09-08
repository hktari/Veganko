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
                await vm.Login();
                await Navigation.PushAsync(new MainPage());
            }
            catch (ServiceException ex)
            {
                await this.Err($"{ex.StatusDescription}: {ex.Response}");
            }
        }
    }
}