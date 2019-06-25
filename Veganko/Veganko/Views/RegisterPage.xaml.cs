using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegisterPage : ContentPage
	{
        private RegisterViewModel vm;

		public RegisterPage ()
		{
			InitializeComponent ();
            BindingContext = this.vm = new RegisterViewModel();
		}

        private async void OnSignUpBtnClicked(object sender, EventArgs args)
        {
            if (await vm.RegisterUser())
            {
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Napaka", "Napaka pri registraciji.", "OK");
            }
        }
	}
}