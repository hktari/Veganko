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
            string str = (string)value;
            if (!string.IsNullOrEmpty(str))
            {
                return ToUpperCase ? str.ToUpper() : str.ToLower();
            }
            else 
            {
                return str;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
