using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Entry), typeof(Veganko.iOS.Renderers.CustomEntryRenderer))]
namespace Veganko.iOS.Renderers
{
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Entry.TextProperty.PropertyName && Control != null)
            {
                Control.SelectedTextRange = Control.GetTextRange(Control.EndOfDocument, Control.EndOfDocument);
            }
        }
    }
}