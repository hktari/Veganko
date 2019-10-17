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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            vm.PageAppeared.Execute(null);
        }
    }
}