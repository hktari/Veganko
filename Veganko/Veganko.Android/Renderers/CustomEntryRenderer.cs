using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Content;
using Android.Graphics;

[assembly: ExportRenderer(typeof(Entry), typeof(Veganko.Droid.Renderers.CustomEntryRenderer))]
namespace Veganko.Droid.Renderers
{
    class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

        }
    }
}
