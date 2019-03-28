using System;
using System.Linq;
using Veganko.Models;
using Veganko.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : TabbedPage
	{
        public MainPage(bool adminAccess = false)
        {
            InitializeComponent();

            if (adminAccess)
            {
                Children.Add(
                    new NavigationPage(new ManageProductsPage())
                    {
                        Title = Device.RuntimePlatform == Device.UWP ? "Manage" : string.Empty,
                        Icon = Device.RuntimePlatform == Device.UWP ? null : "icon.png"
                    });
            }

            var logoutBtn = new ToolbarItem { Text = "Logout" };
            logoutBtn.Clicked += OnLogoutBtnClicked;
            ToolbarItems.Add(logoutBtn);
        }

        private void OnLogoutBtnClicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAccountService>().Logout();
            App.Current.MainPage = new Loginpage();
        }

        private NavigationPage lastPage;

        protected async override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();

            Title = CurrentPage?.Title;

            if (lastPage != null)
            {
                await lastPage.PopToRootAsync();
            }

            lastPage = CurrentPage as NavigationPage;
        }
    }
}