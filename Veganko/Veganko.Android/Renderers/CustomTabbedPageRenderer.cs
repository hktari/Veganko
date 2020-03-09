using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Veganko.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: Xamarin.Forms.ExportRenderer(typeof(TabbedPage), typeof(CustomTabbedPageRenderer))]
namespace Veganko.Droid.Renderers
{
    /// <summary>
    /// Implements the 'pop to root' functionality when tapping on a tab.
    /// </summary>
    public class CustomTabbedPageRenderer : TabbedPageRenderer, TabLayout.IOnTabSelectedListener
    {
        static private MethodInfo basOnTabReselectedMethodInfo;

        static CustomTabbedPageRenderer()
        {
            basOnTabReselectedMethodInfo = typeof(TabbedPageRenderer).GetMethod("TabLayout.IOnTabSelectedListener.OnTabReselected", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public CustomTabbedPageRenderer(Context context) : base(context)
        {
            baseOnTabReselected = basOnTabReselectedMethodInfo?.CreateDelegate(typeof(Func<string>), this) as Func<string>;
        }

        private Func<string> baseOnTabReselected;

        async void TabLayout.IOnTabSelectedListener.OnTabReselected(TabLayout.Tab tab)
        {
            baseOnTabReselected?.Invoke();
            if (Element.CurrentPage is NavigationPage navPage)
            {
                await navPage.PopToRootAsync().ConfigureAwait(false);
            }
        }
    }
}