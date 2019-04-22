using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models.User;
using Veganko.ViewModels;
using Veganko.ViewModels.Profile;
using Veganko.Views.Profile;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProfilePage : BaseContentPage
	{
        ProfileViewModel vm;
        public ProfilePage ()
		{
			InitializeComponent ();
            vm = (ProfileViewModel)BindingContext;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            vm.Refresh();
        }

        private async void OnBackgroundImageTap(object sender, EventArgs arg)
        {
            await Navigation.PushModalAsync(
                new SelectBackgroundPage(new BackgroundImageViewModel(vm.User.ProfileBackgroundId)));
        }
    }
}