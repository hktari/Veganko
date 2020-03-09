using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Veganko.Validations
{
    public class IsImageNotEmpty : IValidationRule<ImageSource>
    {
        public string ValidationMessage { get ; set ; }

        public bool Check(ImageSource value)
        {
            return value != null;
        }
    }
}
