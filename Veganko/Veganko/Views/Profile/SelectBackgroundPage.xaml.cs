using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models.ViewModels.Images;
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
            BindingContext = this.vm = vm;
		}

        private async void OnSaveClicked(object sender, EventArgs arg)
        {
            await vm.Save();
            await Navigation.PopModalAsync();
        }

        private void OnImageClicked(object sender, EventArgs e)
        {
            vm.SelectBackground((SelectableImageId)((BindableObject)sender).BindingContext);
        }
    }
}