using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Extensions;
using Veganko.Models.User;
using Veganko.Services;
using Veganko.Services.Auth;
using Veganko.Services.Http;
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
        public ProfilePage()
        {
            InitializeComponent();
            vm = (ProfileViewModel)BindingContext;
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await vm.Refresh();
        }

        private async void OnBackgroundImageTap(object sender, EventArgs arg)
        {
            await Navigation.PushModalAsync(
                new SelectBackgroundPage(new BackgroundImageViewModel(vm.User.ProfileBackgroundId)));
        }

        private async void OnEditAvatarTap(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(
                new SelectAvatarPage(new SelectAvatarViewModel(vm.User.AvatarId)));
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            App.IoC.Resolve<IAuthService>().Logout();
            App.Current.MainPage = new NavigationPage(new Loginpage());
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                await vm.SaveProfile();
                await DisplayAlert("Hi", "Profilna spremenjena !", "OK");

            }
            catch (ServiceException ex)
            {
                await this.Err(ex.StatusDescription);
            }
        }
    }
}