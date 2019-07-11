using System;

using Veganko.Views;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using Veganko.Services;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Net;

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
            HotReloader.Current.Run(this);
#endif
            MainPage = new Loginpage();

            //MainPage = new MainPage();
        }

        public static void Init(IAuthenticate authenticator)
        {
            Authenticator = authenticator;
        }

        protected override void OnStart ()
		{
#if DEBUG
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
