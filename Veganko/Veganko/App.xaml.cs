using System;

using Veganko.Views;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using Veganko.Services;

namespace Veganko
{
	public partial class App : Application
	{
        public static MobileServiceClient MobileService =
            new MobileServiceClient("https://veganko.azurewebsites.net"
        );
        public static IAuthenticate Authenticator { get; private set; }

        public App ()
		{
			InitializeComponent();
            MainPage = new MainPage();
        }

        public static void Init(IAuthenticate authenticator)
        {
            Authenticator = authenticator;
        }

        protected override void OnStart ()
		{
            //if (MobileService.CurrentUser != null)
            //    MainPage = new MainPage();
            //else
            //    MainPage = new Loginpage();
            IAccountService accountService = DependencyService.Get<IAccountService>();
            accountService.CreateAccount("admin", "admin", "");
            accountService.Login("admin", "admin");

            accountService.User.AccessRights = Models.User.UserAccessRights.All;

            MainPage = new MainPage();
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
