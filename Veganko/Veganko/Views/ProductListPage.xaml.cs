using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Extensions;
using Veganko.Models;
using Veganko.Services.Http;
using Veganko.ViewModels;
using Veganko.ViewModels.Products;
using Veganko.ViewModels.Products.Partial;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace Veganko.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProductListPage : BaseContentPage
	{
        ProductListViewModel vm;

		public ProductListPage()
		{
			InitializeComponent ();
            BindingContext = vm = new ProductListViewModel();
        }

        public ProductListPage(ProductListViewModel vm)
        {
            InitializeComponent();
            BindingContext = this.vm = vm;
        }

        void OnProductSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (args.SelectedItem == null)
                return;

            vm.ProductSelectedCommand.Execute((ProductViewModel)args.SelectedItem);

            // Manually deselect item.
            ProductsListView.SelectedItem = null;
        }

        async void OnDeleteProduct(object sender, EventArgs e)
        {
            string result = await DisplayActionSheet("Are you sure you wish to delete this product ?", "Cancel", "Yes");
            if (result == "Yes")
            {
                var mi = ((MenuItem)sender);
                try
                {
                    await vm.DeleteProduct((ProductViewModel)mi.CommandParameter);
                }
                catch (ServiceException ex)
                {
                    await this.Err("Napak pri brisanju: " + ex.Response);
                }
            }
        }

        private void OnSearchTextInputCompleted(object sender, EventArgs e)
        {
            vm.SearchClickedCommand.Execute(null);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (vm.SearchResult == null || vm.SearchResult.Count == 0)
            {
                vm.LoadItemsCommand.Execute(null);
            }
        }
    }
}