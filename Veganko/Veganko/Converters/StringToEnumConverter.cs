using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using Veganko.Extensions;
using Veganko.Models;
using Xamarin.Forms;

namespace Veganko.Converters
{
    public class StringToEnumConverter : IValueConverter
    {
        public bool UseDescriptionAttribute { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum @enum)
            {
                if (UseDescriptionAttribute)
                {
                    return @enum.GetDescription();
                }
                else
                {
                    return Enum.GetName(value.GetType(), value);
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                if (UseDescriptionAttribute)
                {
                    foreach (var @enum in (IEnumerable<ProductType>)Enum.GetValues(targetType))
                    {
                        if (@enum.GetDescription() == (string)value)
                        {
                            return @enum;
                        }
                    }
                }
                return Enum.Parse(targetType, value as string);
            }
            return 0;

        }
    }
}
