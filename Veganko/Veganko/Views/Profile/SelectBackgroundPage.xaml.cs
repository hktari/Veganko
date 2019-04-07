using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.ViewModels.Profile;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views.Profile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SelectBackgroundPage : ContentPage
	{
        private BackgroundImageViewModel vm;

		public SelectBackgroundPage (BackgroundImageViewModel vm)
		{
			InitializeComponent ();
            this.vm = vm;
		}

        private async void OnSaveClicked(object sender, EventArgs arg)
        {
            await vm.Save();
            await Navigation.PopModalAsync();
        }
	}
}