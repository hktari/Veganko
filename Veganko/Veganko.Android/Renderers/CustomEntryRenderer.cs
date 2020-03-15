using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Content;
using Android.Widget;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(Entry), typeof(Veganko.Droid.Renderers.CustomEntryRenderer))]
namespace Veganko.Droid.Renderers
{
    internal class CustomTextView : EditText
    {
        public CustomTextView(Context context) : base(context)
        {
        }
    }

    internal class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            if(Control != null)
            {
                Control.TextChanged -= Control_TextChanged;
            }

            base.OnElementChanged(e);

            Control.TextChanged += Control_TextChanged;
        }

        private void Control_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (Control.Text != null)
            {
                Control.SetSelection(Control.Text.Length);
            }
        }
    }
}
