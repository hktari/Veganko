using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Veganko.Models;

namespace Veganko.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewProductPage : ContentPage
    {
        public Product Product { get; set; }

        public NewProductPage()
        {
            InitializeComponent();

            Product = new Product
            {
                Name = "Product name",
                Description = "This is a description of the product."
            };

            BindingContext = this;
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "AddItem", Product);
            await Navigation.PopModalAsync();
        }
    }
}