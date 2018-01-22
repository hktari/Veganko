using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProductPage : ContentPage
	{
        ProductViewModel vm;

		public ProductPage ()
		{
			InitializeComponent ();
            BindingContext = vm = new ProductViewModel();
        }
        async void OnProductSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Product;
            if (item == null)
                return;

            await Navigation.PushAsync(new ProductDetailPage(new ProductDetailViewModel(item)));

            // Manually deselect item.
            ProductsListView.SelectedItem = null;
        }
        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewProductPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (vm.Products.Count == 0)
                vm.LoadItemsCommand.Execute(null);
        }
    }
}