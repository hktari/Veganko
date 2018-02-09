using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

[assembly: Xamarin.Forms.ExportRenderer(typeof(Xamarin.Forms.Button), typeof(CL_HVAC.Droid.FlatButtonRenderer))]
namespace CL_HVAC.Droid
{
	public class FlatButtonRenderer : Xamarin.Forms.Platform.Android.ButtonRenderer
    {
		protected override void OnDraw(Android.Graphics.Canvas canvas)
		{
			base.OnDraw(canvas);
		}
	}
}