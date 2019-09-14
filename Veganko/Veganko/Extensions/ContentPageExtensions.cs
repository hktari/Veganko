using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Veganko.Extensions
{
    public static class ContentPageExtensions
    {
        public static Task Err(this ContentPage page, string message)
        {
            return page.DisplayAlert("Napaka", message, "OK");
        }

        public static Task Inform(this ContentPage page, string message)
        {
            return page.DisplayAlert("Sporočilo", message, "OK");
        }
    }
}
