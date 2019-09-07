﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Extensions;
using Veganko.Services.Http;
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
            try
            {
                if (vm.Password != vm.ConfirmPassword)
                {
                    await this.Err("Gesla se ne ujemata.");
                }

                await vm.RegisterUser();
                await Navigation.PopAsync();
            }
            catch (ServiceException ex)
            {

                await this.Err("Napaka pri registraciji: " + ex.Response);
            }
        }
	}
}