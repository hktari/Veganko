using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Veganko.Droid.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("Veganko")]
[assembly: ExportEffect(typeof(CustomEntryEffect), nameof(CustomEntryEffect))]
namespace Veganko.Droid.Effects
{
    class CustomEntryEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            if (Control != null)
            {
                Control.Background.Mutate().SetColorFilter(global::Android.Graphics.Color.White, PorterDuff.Mode.Clear);
            }
        }

        protected override void OnDetached()
        {
            // Nothing
        }
    }
}