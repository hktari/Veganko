using System;
using System.Collections.Generic;
using Veganko.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Product = Veganko.Models.Product;
using System.Linq;
using Veganko.Extensions;
using System.Collections.ObjectModel;
using Veganko.ViewModels;
using Plugin.Media;
using XamarinImageUploader;
using Veganko.Other;
using Veganko.Services.Http;

namespace Veganko.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewProductPage : BaseContentPage
    {
        public EventHandler ScanClicked;

        NewProductViewModel vm;
        
        public NewProductPage()
        {
            InitializeComponent();
            
            BindingContext = vm = new NewProductViewModel();
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            var mainPage = App.Current.MainPage as TabbedPage;
            var productsNavPage = mainPage.Children[0];

            mainPage.CurrentPage = productsNavPage;

            var productsVM = (ProductViewModel)((ProductPage)((NavigationPage)productsNavPage).CurrentPage).BindingContext;
            vm.Product.ProductClassifiers = vm.ProductClassifiers;
            vm.Product.Type = vm.SelectedProductType;

            try
            {
                await productsVM.AddNewProduct(vm.Product);
            }
            catch (ServiceException ex)
            {
                await this.Err("Napak pri dodajanju: " + ex.Response);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            vm.PageAppeared.Execute(null);
        }
    }
}