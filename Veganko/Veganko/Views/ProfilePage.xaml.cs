using Veganko.Extensions;
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
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
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

            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);
            vm = (ProfileViewModel)BindingContext;
        }
        protected override async void CustomOnAppearing()
        {
            await vm.Refresh();
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            App.IoC.Resolve<IAuthService>().Logout();
            App.Current.MainPage = new Xamarin.Forms.NavigationPage(new Loginpage());
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                await vm.SaveProfile();
                await this.Inform("Profilna spremenjena !");

            }
            catch (ServiceException ex)
            {
                await this.Err(ex.StatusCodeDescription);
            }
        }
    }
}