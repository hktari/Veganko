using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Veganko.Models;
using Xamarin.Forms;

namespace Veganko.Converters
{
    public class StringToEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum)
            {
                return Enum.GetName(typeof(ProductClassifier), value);
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                if (Enum.TryParse<ProductClassifier>(value as string, out var result))
                    return result;
            }
            return 0;

        }
    }
}
