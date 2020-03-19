using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Services.Http;
using Veganko.Services.Http.Errors;
using Xamarin.Forms;

namespace Veganko.Extensions
{
    public static class ContentPageExtensions
    {
        /// <summary>
        /// Checks whether the remote has been reached and gives feedback accordingly.
        /// </summary>
        public static Task Err(this ContentPage page, string message, ServiceException ex)
        {
            if (!ex.HasRemoteBeenReached)
            {
                return Err(page, Other.Strings.ServiceUnreachableErr);
            }
            else 
            {
                return Err(page, message);
            }
        }

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
