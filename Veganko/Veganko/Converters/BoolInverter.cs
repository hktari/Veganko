using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Veganko.Converters
{
    public class BoolInverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() != typeof(bool))
                throw new ArgumentException("BoolInverter works only with values of type bool !");
            var flag = (bool)value;
            return !flag;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() != typeof(bool))
                throw new ArgumentException("BoolInverter works only with values of type bool !");
            var flag = (bool)value;
            return !flag;
        }
    }
}
