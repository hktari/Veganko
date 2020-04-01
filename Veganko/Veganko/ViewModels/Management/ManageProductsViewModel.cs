using Autofac;
using System.Collections.Generic;
using Veganko.Services.Products.ProductModRequests;
using Veganko.ViewModels.Products.ModRequests.Partial;

namespace Veganko.ViewModels.Management
{
    public class ManageProductsViewModel : BaseViewModel
    {

        private List<ProductModRequestViewModel> productModReqs;
        public List<ProductModRequestViewModel> ProductModReqs
        {
            get => productModReqs;
            set => SetProperty(ref productModReqs, value);
        }

        private IProductModRequestService ProductModRequestService => App.IoC.Resolve<IProductModRequestService>();

        public override void OnPageAppearing()
        {
            base.OnPageAppearing();
            
            ProductModRequestService.AllAsync()
        }
        //protected override Task OnProductSelected(ProductViewModel product)
        //{
        //    throw new NotImplementedException();
        //    // TODO: move to product view model.
        //    Product productModel = new Product();
        //    product.MapToModel(productModel);
        //    //return App.Navigation.PushAsync(new ApproveProductPage(new ProductModRequestDetailViewModel(productModel)));
        //}

        //protected async override Task<List<ProductViewModel>> GetProducts()
        //{
        //    return new List<ProductViewModel>();
        //    //IEnumerable<Product> products = await productService.GetUnapprovedAsync(true);
        //    //return new List<ProductViewModel>(
        //        //products.Select(p => new ProductViewModel(p)));
        //}
    }
}
