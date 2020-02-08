using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(Veganko.iOS.Effects.CustomEntryEffect), "CustomEntryEffect")]
namespace Veganko.iOS.Effects
{
    public class CustomEntryEffect : PlatformEffect
    {
        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if(Control?.Layer != null)
            {
                Control.Layer.BorderColor = Color.White.ToCGColor();
                // Thickness of the Border Width  
                Control.Layer.BorderWidth = 1;
                Control.ClipsToBounds = true;
                Control.TintColor = Color.FromHex("#448e00").ToUIColor();
            }
        }
        protected override void OnAttached()
        {
        }

        protected override void OnDetached()
        {
        }
    }
}