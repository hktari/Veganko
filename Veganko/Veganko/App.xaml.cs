using System;

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

namespace Veganko
{
    public partial class App : Application
    {
        public static MobileServiceClient MobileService =
            new MobileServiceClient("https://veganko.azurewebsites.net"
        );
        public static IAuthenticate Authenticator { get; private set; }

        public static IContainer IoC { get; private set; }

        public static INavigation Navigation
        {
            get
            {
                return (App.Current.MainPage as TabbedPage)?.CurrentPage?.Navigation
                    ?? throw new Exception("No navigation available on current page.");
            }
        }

        public static ContentPage CurrentPage
        {
            get
            {
                Page tabPage = (App.Current.MainPage as TabbedPage)?.CurrentPage;
                ContentPage curPage;
                if (tabPage is NavigationPage navigationPage)
                {
                    curPage = navigationPage?.CurrentPage as ContentPage;
                }
                else
                {
                    curPage = tabPage as ContentPage;
                }

                return curPage ?? throw new Exception("Failed to get current page.");
            }
        }

        public App ()
		{
			InitializeComponent();

            SetupDependencies();

//#if DEBUG && __ANDROID__
//            HotReloader.Current.Run(this);
//#endif
            // UWP requirement
            MainPage = new NavigationPage(new Loginpage());
        }

        private void SetupDependencies()
        {   
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<RestService>()
                .As<IRestService>()
                .SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();

            //builder.RegisterType<AccountService>().As<IAccountService>().SingleInstance();
            //builder.RegisterType<ProductDataStore>().As<IProductService>().SingleInstance();
            //builder.RegisterType<CommentsService>().As<ICommentsService>().SingleInstance();
            //builder.RegisterType<UserService>().As<IUserService>().SingleInstance();
            //builder.RegisterType<AuthService>().As<IAuthService>().SingleInstance();

            builder.RegisterType<MockAccountService>().As<IAccountService>().SingleInstance();
            builder.RegisterType<MockProductDataStore>().As<IProductService>().SingleInstance();
            builder.RegisterType<MockCommentsService>().As<ICommentsService>().SingleInstance();
            builder.RegisterType<MockUserService>().As<IUserService>().SingleInstance();
            builder.RegisterType<MockAuthService>().As<IAuthService>().SingleInstance();

            IoC = builder.Build();
        }

        public static void Init(IAuthenticate authenticator)
        {
            Authenticator = authenticator;
        }

        protected async override void OnStart ()
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
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
