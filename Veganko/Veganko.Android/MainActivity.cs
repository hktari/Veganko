using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ImageCircle.Forms.Plugin.Droid;
using ZXing.Mobile;
using Microsoft.WindowsAzure.MobileServices;
using Veganko.Services;
using System.Threading.Tasks;
using System.Runtime.Remoting.Contexts;
using FFImageLoading.Forms.Platform;
using Android.Content;
using Android.Provider;
using Java.IO;
using Android.Support.V4.Content;
using Java.Text;
using Java.Util;
using Android.Graphics;
using Android.Util;
using System.IO;
using Java.Nio;
using System.Runtime.InteropServices;
using Android.Media;
using System.Collections.Generic;

namespace Veganko.Droid
{
    [Activity(Label = "Veganko", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public partial class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IAuthenticate
    {
        public static int OSVersion;

        public static MainActivity Context { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(bundle);

            OSVersion = (int)Android.OS.Build.VERSION.SdkInt;

            Context = this;
            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.Essentials.Platform.Init(this, bundle);
            ImageCircleRenderer.Init();
            CachedImageRenderer.Init(true);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            // Initialize the scanner first so it can track the current context
            MobileBarcodeScanner.Initialize(this.Application);
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, bundle);
            CurrentPlatform.Init();
            // Initialize the authenticator before loading the app.
            App.Init((IAuthenticate)this);

            LoadApplication(new App());
        }
        
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            OnRequestPermissionsResult_PhotoPicking(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed()
        {
            // Prevent users from accidentally closing the app via android back button.
            // This is handled only when TabbedPage is shown and there is either only the root page on the nav stack
            // Special handling is done for root pages which have no nav stack and use modals.
            if (App.Current.MainPage is Views.MainPage && 
                (App.Navigation.NavigationStack.Count < 2 && App.Navigation.ModalStack.Count == 0))
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Sporočilo");
                alert.SetMessage("Želiš zapustiti vegankona ?");
                alert.SetPositiveButton("Ja", (senderAlert, args) =>
                {
                    this.FinishAffinity();
                });

                alert.SetNegativeButton("Ostani", (s, args) => { });

                alert.Show();
            }
            else 
            {
                base.OnBackPressed();
            }
        }

        //#region LocationPicking
        //int AUTOCOMPLETE_REQUEST_CODE = 1;
        //public void StartLocationPicking()
        //{ 
        
        //    // Set the fields to specify which types of place data to
        //    // return after the user has made a selection.
        //    List<Place.Field> fields = Arrays.AsList(Place.Field.ID, Place.Field.NAME);     

        //    // Start the autocomplete intent.
        //    Intent intent = new Autocomplete.IntentBuilder(
        //            AutocompleteActivityMode.FULLSCREEN, fields)
        //            .build(this);
        //    startActivityForResult(intent, AUTOCOMPLETE_REQUEST_CODE);
        //}

        //#endregion

        #region FB_Auth
        // Define a authenticated user.
        private MobileServiceUser user;
        
        public async Task<bool> Authenticate()
        {
            var success = false;
            var message = string.Empty;
            try
            {
                // Sign in with Facebook login using a server-managed flow.
                user = await App.MobileService.LoginAsync(this,
                    MobileServiceAuthenticationProvider.Facebook, "facebook");
                if (user != null)
                {
                    message = string.Format("you are now signed-in as {0}.",
                        user.UserId);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            // Display the success or failure message.
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetMessage(message);
            builder.SetTitle("Sign-in result");
            builder.Create().Show();

            return success;
        }
        #endregion
    }
}

