using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.Widget;
using Veganko.Controls;
using Veganko.Droid.CustomSpinner;
using Veganko.Models;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Veganko.Droid.Renderers;
using Veganko.Extensions;
using Veganko.Converters;
using System.Collections.Generic;

[assembly: ExportRenderer(typeof(DroidProductTypeSpinner), typeof(DroidProductTypeSpinnerRenderer))]
namespace Veganko.Droid.Renderers
{
    public class DroidProductTypeSpinnerRenderer : ViewRenderer<DroidProductTypeSpinner, Spinner>
    {
        private CustomSpinnerAdapter adapter;
        private StringToEnumConverter stringEnumConverter = new StringToEnumConverter() { UseDescriptionAttribute = true };
        private Spinner spinner;

        public DroidProductTypeSpinnerRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<DroidProductTypeSpinner> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                spinner = new Spinner(Context);

                if (adapter != null)
                {
                    adapter.ItemSelected -= OnItemSelected;
                }

                var images = new List<int>
                {
                    Resource.Drawable.ico_search,
                    Resource.Drawable.ico_help,
                    Resource.Drawable.ico_food,
                    Resource.Drawable.ico_beverages,
                    Resource.Drawable.ico_cosmetics,
                };
                var descriptions = EnumExtensionMethods.GetDescriptions(ProductType.NOT_SET);

                if (Element.ExcludeNoFilterValue)
                {
                    images.RemoveAt(0);
                    descriptions.RemoveAt(0);
                }

                adapter = new CustomSpinnerAdapter(Context, images.ToArray(), descriptions);
                adapter.ItemSelected += OnItemSelected;
                spinner.OnItemSelectedListener = adapter;
                spinner.Adapter = adapter;

                SetNativeControl(spinner);

                UpdateSelectedProductType(Element.SelectedProductType);
            }
        }

        private void OnItemSelected(object sender, string productType)
        {
            Element.SelectedProductType = (ProductType)stringEnumConverter.ConvertBack(productType, typeof(ProductType), null, null);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == DroidProductTypeSpinner.SelectedProductTypeProperty.PropertyName)
            {
                UpdateSelectedProductType(Element.SelectedProductType);
            }
        }

        private void UpdateSelectedProductType(ProductType productType)
        {
            string ptName = (string)stringEnumConverter.Convert(productType, typeof(string), null, null);

            // Get the idx of ptName in adapter.Items
            var selectedIdx = adapter.Items.Select((item, idx) => item == ptName ? idx : -1).First(i => i != -1);

            if (Control.SelectedItemPosition != selectedIdx)
            {
                Control.SetSelection(selectedIdx);
            }
        }
    }
}