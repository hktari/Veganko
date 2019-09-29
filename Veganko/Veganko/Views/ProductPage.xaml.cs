using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Extensions;
using Veganko.Models;
using Veganko.Services.Http;
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

        public ProductPage(ProductViewModel vm)
        {
            InitializeComponent();
            BindingContext = this.vm = vm;
        }

        void OnProductSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Veganko.Models.Product;
            if (item == null)
                return;

            vm.ProductSelectedCommand.Execute(item);

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
                    await vm.DeleteProduct((Veganko.Models.Product)mi.CommandParameter);
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

            vm.LoadItemsCommand.Execute(null);
        }
    }
}