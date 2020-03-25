using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Extensions;
using Veganko.Services.Http;
using Veganko.ViewModels;
using Veganko.ViewModels.Products;
using Veganko.ViewModels.Products.Partial;
using Veganko.Views.Product;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManageProductsPage : BaseContentPage
    {
        ManageProductsViewModel vm;

        public ManageProductsPage()
        {
            InitializeComponent();
            BindingContext = vm = new ManageProductsViewModel();
        }

        protected override void CustomOnAppearing()
        {
            // TODO: uncomment when implementation exists on service side
            //vm.LoadItemsCommand.Execute(null);
        }

        async void OnProductSelected(object sender, SelectedItemChangedEventArgs args)
        {
            throw new NotImplementedException();
            //var item = args.SelectedItem as Veganko.Models.Product;
            //if (item == null)
            //    return;

            //await Navigation.PushAsync(
            //    new ApproveProductPage(new ApproveProductViewModel(item)));

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

    }
}