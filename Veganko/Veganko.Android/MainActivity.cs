﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ImageCircle.Forms.Plugin.Droid;
using ZXing.Mobile;
using Microsoft.WindowsAzure.MobileServices;

[assembly: UsesFeature("android.hardware.camera", Required = false)]
[assembly: UsesFeature("android.hardware.camera.autofocus", Required = false)]
namespace Veganko.Droid
{
    [Activity(Label = "Veganko", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {        
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            ImageCircleRenderer.Init();
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            // Initialize the scanner first so it can track the current context
            MobileBarcodeScanner.Initialize(this.Application);
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;
            CurrentPlatform.Init();

            LoadApplication(new App());
        }
        
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

