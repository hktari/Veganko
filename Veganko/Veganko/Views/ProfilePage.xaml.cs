using Veganko.Extensions;
using Autofac;
using System;
using Veganko.Other;
using Veganko.Services.Auth;
using Veganko.Services.Http;
using Veganko.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using System.Diagnostics;

namespace Veganko.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : BaseContentPage
    {
        private ProfileViewModel vm;
        public ProfilePage()
        {
            InitializeComponent();
            vm = (ProfileViewModel)BindingContext;

            if (Device.RuntimePlatform == Device.iOS)
            {
                Xamarin.Forms.NavigationPage.SetTitleIconImageSource(
                    this.Parent,
                    new FontImageSource { FontFamily = "Material Icons", Glyph = MaterialDesignIcons.AccountCircle });
            }
            else
            {
                //SetBinding(IconImageSourceProperty, new Binding("AvatarImage"));
            }

            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);
        }
        protected override void OnParentSet()
        {
            base.OnParentSet();
            Debug.Assert(Parent != null);
            Parent?.SetBinding(Xamarin.Forms.NavigationPage.IconImageSourceProperty, new Binding("AvatarImage", source: vm));
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