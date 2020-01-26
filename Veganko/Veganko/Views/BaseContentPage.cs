using System;
using System.Collections.Generic;
using System.Text;
using Veganko.ViewModels;
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

        protected BaseViewModel ViewModel => BindingContext as BaseViewModel
            ?? throw new Exception($"Expecting BindingContext to be of type {typeof(BaseViewModel)}");
        
        protected sealed override void OnAppearing()
        {
            base.OnAppearing();
            CustomOnAppearing();
            ViewModel.OnPageAppearing();
        }

        protected override void OnDisappearing()
        {
            ViewModel.OnPageDisappearing();
            CustomOnDisappearing();
            base.OnDisappearing();
        }

        protected virtual void CustomOnDisappearing()
        { 
        }

        protected virtual void CustomOnAppearing()
        { 
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
