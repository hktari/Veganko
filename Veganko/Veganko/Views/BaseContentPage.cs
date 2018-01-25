using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Veganko.Views
{
    public class BaseContentPage : ContentPage
    {
        new public void SendAppearing()
        {
            OnAppearing();
        }

        new public void SendDisappearing()
        {   
            OnDisappearing();
        }
    }
}
