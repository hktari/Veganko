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
#if DEBUG && __ANDROID__
            HotReloader.Current.Start(this);
#endif
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

            //MainPage = new MainPage();
            MainPage = new Loginpage();
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
