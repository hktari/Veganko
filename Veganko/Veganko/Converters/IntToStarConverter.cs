using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Veganko.Converters
{
    public class IntToStarConverter : IValueConverter
    {
        private const char StarUnicode = '\u2605';
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = (int)value;
            string result = "";
            for (int i = 0; i < val; i++)
            {
                result += StarUnicode;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
