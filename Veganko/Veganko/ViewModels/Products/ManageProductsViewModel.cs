using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Common.Models.Products;
using Veganko.Models;
using Veganko.ViewModels.Products.Partial;
using Veganko.Views;
using Veganko.Views.Product;
using Xamarin.Forms;

namespace Veganko.ViewModels.Products
{
    public class ManageProductsViewModel : ProductListViewModel
    {
        protected override Task OnProductSelected(ProductViewModel product)
        {
            throw new NotImplementedException();
            // TODO: move to product view model.
            Product productModel = new Product();
            product.MapToModel(productModel);
            //return App.Navigation.PushAsync(new ApproveProductPage(new ProductModRequestDetailViewModel(productModel)));
        }

        protected async override Task<List<ProductViewModel>> GetProducts()
        {
            return new List<ProductViewModel>();
            //IEnumerable<Product> products = await productService.GetUnapprovedAsync(true);
            //return new List<ProductViewModel>(
                //products.Select(p => new ProductViewModel(p)));
        }
    }
}
