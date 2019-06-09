using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#if __ANDROID__
using Xamarin.Forms.Platform.Android;
using Android.Widget;
using Veganko.Droid.CustomSpinner;
#endif
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
#if __ANDROID__
            Spinner spinner = new Spinner(Veganko.Droid.MainActivity.Context);
            CustomSpinnerAdapter adapter = new CustomSpinnerAdapter(Droid.MainActivity.Context, 
                new int[] 
                {
                    Veganko.Droid.Resource.Drawable.ico_search,
                    Veganko.Droid.Resource.Drawable.ico_food,
                    Veganko.Droid.Resource.Drawable.ico_beverages,
                    Veganko.Droid.Resource.Drawable.ico_cosmetics,
                },
                new string[]
                {
                    "none",
                    "hrana",
                    "pijača",
                    "kozmetika"
                });
            adapter.ItemSelected += (s, productType) => Console.WriteLine(productType);
            spinner.OnItemSelectedListener = adapter;
            spinner.Adapter = adapter;
            pickerRoot.Content = spinner.ToView();
            //pickerRoot.Content = 
#endif
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

        async void OnDeleteProduct(object sender, EventArgs e)
        {
            string result = await DisplayActionSheet("Are you sure you wish to delete this product ?", "Cancel", "Yes");
            if (result == "Yes")
            {
                var mi = ((MenuItem)sender);
                await vm.DeleteProduct((Veganko.Models.Product)mi.CommandParameter);
            }
        }

        private void OnSearchTextInputCompleted(object sender, EventArgs e)
        {
            vm.SearchClickedCommand.Execute(null);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //if (vm.Products == null || vm.Products.Count == 0)
            await vm.RefreshProducts().ConfigureAwait(false);
        }
    }
}