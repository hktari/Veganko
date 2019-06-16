using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Veganko.Controls;
using Veganko.Droid.CustomSpinner;
using Veganko.Models;
using Xamarin.Android;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Veganko.Droid.Renderers;

[assembly: ExportRenderer(typeof(DroidProductTypeSpinner), typeof(DroidProductTypeSpinnerRenderer))]
namespace Veganko.Droid.Renderers
{
    public class DroidProductTypeSpinnerRenderer : ViewRenderer<DroidProductTypeSpinner, Spinner>
    {
        private CustomSpinnerAdapter adapter;

        public DroidProductTypeSpinnerRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<DroidProductTypeSpinner> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var spinner = new Spinner(Context);
                adapter = new CustomSpinnerAdapter(Context,
                    new int[]
                    {
                        Veganko.Droid.Resource.Drawable.ico_search,
                        Veganko.Droid.Resource.Drawable.ico_food,
                        Veganko.Droid.Resource.Drawable.ico_beverages,
                        Veganko.Droid.Resource.Drawable.ico_cosmetics,
                    },
                    Enum.GetNames(typeof(ProductType)).ToList());
                adapter.ItemSelected += OnItemSelected;
                spinner.OnItemSelectedListener = adapter;
                spinner.Adapter = adapter;

                SetNativeControl(spinner);
            }
        }

        private void OnItemSelected(object sender, string productType)
        {
            Element.SelectedProductType = (ProductType)Enum.Parse(typeof(ProductType), productType);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == DroidProductTypeSpinner.SelectedProductTypeProperty.PropertyName)
            {
                var ptName = Enum.GetName(typeof(ProductType), Element.SelectedProductType);
                // Get the idx of ptName in adapter.Items
                var selectedIdx = adapter.Items.Select((item, idx) => item == ptName ? idx : -1).First(i => i != -1);

                if (Control.SelectedItemPosition != selectedIdx)
                {
                    Control.SetSelection(selectedIdx);
                }

            }
        }
    }
}