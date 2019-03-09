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
	public partial class ProductPage : BaseContentPage
	{
        ProductViewModel vm;

		public ProductPage ()
		{
			InitializeComponent ();
            BindingContext = vm = new ProductViewModel();
        }
        async void OnProductSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Veganko.Models.Product;
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

        async void OnDeleteProduct(object sender, EventArgs e)
        {
            string result = await DisplayActionSheet("Are you sure you wish to delete this product ?", "Cancel", "Yes");
            if (result == "Yes")
            {
                var mi = ((MenuItem)sender);
                await vm.DeleteProduct((Veganko.Models.Product)mi.CommandParameter);
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            vm.SetUserRights();

            if (vm.Products == null || vm.Products.Count == 0)
                await vm.RefreshProducts().ConfigureAwait(false);
        }
    }
}