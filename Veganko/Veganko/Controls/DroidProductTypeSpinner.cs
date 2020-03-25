using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Veganko.Models;
using System.Linq;
using Veganko.Common.Models.Products;

namespace Veganko.Controls
{
    public class DroidProductTypeSpinner : View
    {
        public static readonly BindableProperty SelectedProductTypeProperty = BindableProperty.Create(
                nameof(SelectedProductType), typeof(ProductType), typeof(DroidProductTypeSpinner), default(ProductType), BindingMode.TwoWay);

        public DroidProductTypeSpinner()
        {
        }
        public DroidProductTypeSpinner(bool excludeNoFilterValue)
        {
            ExcludeNoFilterValue = excludeNoFilterValue;
        }

        public ProductType SelectedProductType
        {
            get => (ProductType)GetValue(SelectedProductTypeProperty);
            set => SetValue(SelectedProductTypeProperty, value);
        }
        public bool ExcludeNoFilterValue { get; }
    }
}
