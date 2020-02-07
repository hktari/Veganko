using Autofac;
using System;
using Veganko.Extensions;
using Veganko.Other;
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
        private ProfileViewModel vm;
        public ProfilePage()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.iOS)
            {
                IconImageSource = new FontImageSource { FontFamily = "Material Icons", Glyph = MaterialDesignIcons.AccountCircle };
            }
            else
            {
                SetBinding(IconImageSourceProperty, new Binding("AvatarImage"));
            }

            vm = (ProfileViewModel)BindingContext;
        }
        protected override async void CustomOnAppearing()
        {
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
                await this.Err(ex.StatusCodeDescription);
            }
        }
    }
}