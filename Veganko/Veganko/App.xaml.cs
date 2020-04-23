﻿using System;

using Veganko.Views;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using Veganko.Services;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Net;
using Veganko.Services.Http;
using Autofac;
using Veganko.Services.Users;
using Veganko.Services.Comments;
using Veganko.Services.Auth;
using Veganko.Services.Logging;
using Veganko.Services.ImageManager;
using System.Net.Http;
using Veganko.Services.Products.Stores;
using Veganko.Services.DB;
using Veganko.Services.Products;
using Veganko.Services.Products.ProductModRequests;
using Veganko.Services.Resources;
using Veganko.Services.Storage;

namespace Veganko
{
    public partial class App : Application
    {
#if __ANDROID__
        public const string AssemblyNamespacePrefix = "Veganko.Droid.";
#elif __IOS__
        public const string AssemblyNamespacePrefix = "Veganko.iOS.";
#elif WINDOWS_UWP
        public const string AssemblyNamespacePrefix = "Veganko.UWP.";
#else
        public const string AssemblyNamespacePrefix = "UnitTests.UWP.";
#endif

        public static MobileServiceClient MobileService =
            new MobileServiceClient("https://veganko.azurewebsites.net"
        );
        public static IAuthenticate Authenticator { get; private set; }

        public static IContainer IoC { get; private set; }

        public static INavigation Navigation
        {
            get
            {
                return CurrentPage.Navigation
                    ?? throw new Exception("No navigation available on current page.");
            }
        }

        public static ContentPage CurrentPage
        {
            get
            {
                ContentPage curPage = null;

                if (App.Current.MainPage is TabbedPage tabbedPage)
                {
                    if (tabbedPage.CurrentPage is NavigationPage navigationPage)
                    {
                        curPage = navigationPage?.CurrentPage as ContentPage;
                    }
                    else
                    {
                        curPage = tabbedPage.CurrentPage as ContentPage;
                    }
                }
                else if (App.Current.MainPage is NavigationPage navPage)
                {
                    curPage = navPage.CurrentPage as ContentPage;
                }
                
                return curPage ?? throw new Exception("Failed to get current page.");
            }
        }

        public App ()
		{
			InitializeComponent();

#if RELEASE
            SetupDependencies(mock: false);
#else
            SetupDependencies(mock: false);
#endif

            //#if DEBUG && __ANDROID__
            //            HotReloader.Current.Run(this);
            //#endif
            // UWP requirement
            MainPage = new NavigationPage(new Loginpage());
        }

        public static void SetCurrentTab(int tabIdx)
        {
            ((MainPage)App.Current.MainPage).SetCurrentTab(tabIdx);
        }

        public static void SetupDependencies(bool mock)
        {   
            ContainerBuilder builder = new ContainerBuilder();

            if (!mock)
            {
                builder.RegisterType<RestService>()
                    .As<IRestService>()
                    .SingleInstance()
                    .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

                builder.RegisterType<AccountService>().As<IAccountService>().SingleInstance();
                builder.RegisterType<ProductDataStore>().As<IProductService>().SingleInstance();
                builder.RegisterType<CommentsService>().As<ICommentsService>().SingleInstance();
                builder.RegisterType<UserService>().As<IUserService>().SingleInstance();
                builder.RegisterType<AuthService>().As<IAuthService>().SingleInstance();
                builder.RegisterType<StoresService>().As<IStoresService>().SingleInstance();
                builder.RegisterType<ProductDBService>().As<IProductDBService>().SingleInstance();
            }
            else 
            {
                builder.RegisterType<MockStoresService>().As<IStoresService>().SingleInstance();
                builder.RegisterType<MockAccountService>().As<IAccountService>().SingleInstance();
                builder.RegisterType<MockProductDataStore>().As<IProductService>().SingleInstance();
                builder.RegisterType<MockCommentsService>().As<ICommentsService>().SingleInstance();
                builder.RegisterType<MockUserService>().As<IUserService>().SingleInstance();
                builder.RegisterType<MockAuthService>().As<IAuthService>().SingleInstance();
                //builder.RegisterType<MockProductDBService>().As<IProductDBService>().SingleInstance();
                builder.RegisterType<ProductDBService>().As<IProductDBService>().SingleInstance();
            }

            builder.RegisterType<XamarinPreferences>().As<IPreferences>().SingleInstance();
            builder.RegisterType<ResourceProvider>().As<IResourceProvider>().SingleInstance();
            builder.RegisterType<ProductModRequestService>().As<IProductModRequestService>().SingleInstance();
            builder.RegisterType<ProductHelper>().As<IProductHelper>().SingleInstance();
            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();

            builder.RegisterType<ImageProcessor>().As<IImageProcessor>().SingleInstance();

            IoC = builder.Build();
        }

        public static void Init(IAuthenticate authenticator)
        {
            Authenticator = authenticator;
        }

        protected override void OnStart ()
		{
#if !DEBUG
            AppCenter.Start(
                "android=daa6adb5-45f6-42a5-9612-34de5f472a92;",
                typeof(Analytics),
                typeof(Crashes));
#endif

            //if (MobileService.CurrentUser != null)
            //    MainPage = new MainPage();
            //else
            //    MainPage = new Loginpage();

            //MainPage = new MainPage();
        }

        protected override void OnSleep ()
		{
        }

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
