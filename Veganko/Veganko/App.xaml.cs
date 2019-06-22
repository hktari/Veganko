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
            HotReloader.Current.Run(this,
                new HotReloader.Configuration
                {
                    ExtensionIpAddress = IPAddress.Parse("192.168.1.243"),
                    DeviceUrlPort = 8000,
                    ExtensionAutoDiscoveryPort = 15000
                });
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
            AppCenter.Start(
                "android=daa6adb5-45f6-42a5-9612-34de5f472a92;",
                typeof(Analytics),
                typeof(Crashes));      
            
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
