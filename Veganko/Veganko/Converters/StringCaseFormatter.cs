using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Veganko.Converters
{
    public class StringCaseFormatter : IValueConverter
    {
        public bool ToUpperCase { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return ToUpperCase ? str.ToUpper() : str.ToLower();
            }

            throw new Exception("Invalid value type!");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
