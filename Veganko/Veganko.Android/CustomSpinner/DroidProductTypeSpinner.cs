using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Veganko.Models;
using Xamarin.Forms.Platform.Android;
using Android.Widget;
using Veganko.Droid.CustomSpinner;
using System.Linq;

namespace Veganko.Droid.CustomSpinner
{
    public class DroidProductTypeSpinner : ContentView
    {
        public static readonly BindableProperty SelectedProductTypeProperty = BindableProperty.Create(
                nameof(SelectedProductType), typeof(ProductType), typeof(DroidProductTypeSpinner), default(ProductType), BindingMode.TwoWay, propertyChanged: SelectedPropertyTypeChanged);

        private CustomSpinnerAdapter adapter;
        private Spinner spinner;

        private static void SelectedPropertyTypeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var dpts = (DroidProductTypeSpinner)bindable;
            var ptName = Enum.GetName(typeof(ProductType), newValue);
            // Get the idx of ptName in adapter.Items
            var selectedIdx = dpts.adapter.Items.Select((item, idx) => item == ptName ? idx : -1).First(i => i != -1);
            dpts.spinner.SetSelection(selectedIdx);
        }

        public ProductType SelectedProductType
        {
            get => (ProductType)GetValue(SelectedProductTypeProperty);
            set => SetValue(SelectedProductTypeProperty, value);
        }

        public DroidProductTypeSpinner()
        {
            Spinner spinner = new Spinner(Veganko.Droid.MainActivity.Context);
            adapter = new CustomSpinnerAdapter(Droid.MainActivity.Context,
                new int[]
                {
                    Veganko.Droid.Resource.Drawable.ico_search,
                    Veganko.Droid.Resource.Drawable.ico_food,
                    Veganko.Droid.Resource.Drawable.ico_beverages,
                    Veganko.Droid.Resource.Drawable.ico_cosmetics,
                },
                Enum.GetNames(typeof(ProductType)));
            adapter.ItemSelected += (s, productType) => SelectedProductType = (ProductType)Enum.Parse(typeof(ProductType), productType);
            spinner.OnItemSelectedListener = adapter;
            spinner.Adapter = adapter;
            Content = spinner.ToView();
        }
    }
}
