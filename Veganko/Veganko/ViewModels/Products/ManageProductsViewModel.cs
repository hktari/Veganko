using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Views;
using Veganko.Views.Product;
using Xamarin.Forms;

namespace Veganko.ViewModels.Products
{
    public class ManageProductsViewModel : ProductViewModel
    {
        protected override Task OnProductSelected(Product product)
        {
            return App.Navigation.PushAsync(new ApproveProductPage(new ApproveProductViewModel(product)));
        }

        protected async override Task<List<Product>> GetProducts()
        {
            return new List<Product>(
                await productService.GetUnapprovedAsync(true));
        }
    }
}
