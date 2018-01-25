using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Veganko.Models;
using System.Linq;
using Veganko.Extensions;

namespace Veganko.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewProductPage : BaseContentPage
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
            TypePicker.ItemsSource = Enum.GetNames(typeof(ProductType)).Select(b => b.SplitCamelCase()).ToList();
            ProductClassifierView.Source = new Dictionary<ProductClassifier, string>
            {
                { ProductClassifier.VEGETARIAN, "ico_vegetarian.png" },
                { ProductClassifier.VEGAN, "ico_vegan.png" },
                { ProductClassifier.PESCETARIAN, "ico_pescetarian.png" },
                { ProductClassifier.GLUTENFREE, "ico_gluten_free.png" },
                { ProductClassifier.RAW_VEGAN, "ico_raw_vegan.png" }
            };
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "AddItem", Product);
            await Navigation.PopModalAsync();
        }
    }
}