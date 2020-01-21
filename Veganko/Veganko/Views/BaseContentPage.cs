using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Veganko.Views
{
    public class BaseContentPage : ContentPage
    {
        public BaseContentPage(bool useNavBar = false)
        {
            if (!useNavBar)
            {
                NavigationPage.SetHasNavigationBar(this, false);
            }
        }

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
