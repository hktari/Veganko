using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Veganko.Models.User;
using Xamarin.Forms;

namespace Veganko.Converters
{
    public class UACConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var mask = (UserAccessRights)parameter;
                var uac = (UserAccessRights)value;
                return (uac & mask) == mask;
            }
            catch (InvalidCastException ex)
            {
                Debug.WriteLine("Can't convert to UserAccessRights ! " + ex.Message);
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
