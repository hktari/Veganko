using Autofac;
using System;
using System.Linq;
using Veganko.Models;
using Veganko.Other;
using Veganko.Services;
using Veganko.Services.Resources;
using Veganko.ViewModels;
using Veganko.ViewModels.Products;
using Veganko.Views.Management;
using Veganko.Views.Profile;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : TabbedPage
	{
        public MainPage(bool isManager = false)
        {
            InitializeComponent();

            // TODO: uncomment this when members are going to be introduced
            //if (isManager)
            //{
            //    Children.Add(
            //        new NavigationPage(new ProductListPage(new ManageProductsViewModel()))
            //        {
            //            Title = Device.RuntimePlatform == Device.UWP ? "Manage" : string.Empty,
            //            Icon = Device.RuntimePlatform == Device.UWP ? null : "icon.png"
            //        });
            //}
            if (isManager)
            {
                var resProvider = App.IoC.Resolve<IResourceProvider>();
                //Children.Add(new NavigationPage(new ManagementPage())
                //{
                //    IconImageSource = new FontImageSource
                //    {
                //        Glyph = MaterialDesignIcons.SupervisorAccount,
                //        Color = Color.Black,
                //        FontFamily = resProvider.GetByKey<OnPlatform<string>>("MaterialDesignFont")
                //    }
                //});
                Children.Add(new NavigationPage(new ProfileManagerPage()));
                // Only manager and admin
                //Children.Add(new NavigationPage(new ManageUsersPage())
                //{
                //    IconImageSource = new FontImageSource
                //    {
                //        Glyph = MaterialDesignIcons.SupervisorAccount,
                //        Color = Color.Black,
                //        FontFamily = resProvider.GetByKey<OnPlatform<string>>("MaterialDesignFont")
                //    }
                //});
            }
            else
            {
                Children.Add(new NavigationPage(new ProfilePage()));
            }
        }

        private NavigationPage lastPage;

        public void SetCurrentTab(int tabIdx)
        {
            var page = Children.ElementAtOrDefault(tabIdx);
            CurrentPage = page ?? throw new ArgumentException("Invalid page idx.", nameof(tabIdx));
        }

        protected async override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();

            if (lastPage != null)
            {
                await lastPage.PopToRootAsync();
            }

            lastPage = CurrentPage as NavigationPage;
        }
    }
}