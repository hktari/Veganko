using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Veganko.Views
{
    public class BaseContentPage : ContentPage
    {
        public BaseContentPage()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                NavigationPage.SetHasNavigationBar(this, false);
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                Padding = new Thickness(0, 34, 0, 0);
            }
        }

        //new public void SendAppearing()
        //{
        //    OnAppearing();
        //}

        //new public void SendDisappearing()
        //{   
        //    OnDisappearing();
        //}
    }
}
