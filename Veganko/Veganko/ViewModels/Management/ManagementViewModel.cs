using Autofac;
using System.Collections.Generic;
using System.Linq;
using Veganko.Common.Models.Products;
using Veganko.Extensions;
using Veganko.Models;
using Veganko.Services.Http;
using Veganko.Services.Products.ProductModRequests;
using Veganko.ViewModels.Products.ModRequests.Partial;
using Veganko.ViewModels.Products.Partial;
using Xamarin.Forms;
using Xamarin.Forms.Extended;

namespace Veganko.ViewModels.Management
{
    public class ManagementViewModel : BaseViewModel
    {
        private const int PageSize = 30;
        
        public ManagementViewModel()
        {
            ProductModReqs = new InfiniteScrollCollection<ProductModRequestViewModel>()
            {
                OnCanLoadMore = () => CurrentPage <= TotalPages,
                OnLoadMore = async () =>
                {
                    try
                    {
                        IsBusy = true;
                        // load the next page
                        PagedList<ProductModRequestDTO> page = await ProductModRequestService.AllAsync(CurrentPage + 1, PageSize, state: ProductModRequestState.Pending);
                        TotalPages = page.TotalPages;

                        // return the items that need to be added
                        return page?.Items.Select(p => new ProductModRequestViewModel(p));
                    }
                    catch (ServiceException sex)
                    {
                        await App.CurrentPage.Err("Napaka pri nalaganju.", sex);
                        return ProductModReqs;
                    }
                    finally
                    {
                        IsBusy = false;
                    }
                }
            };
        }

        public int CurrentPage => ProductModReqs.Count / PageSize;
        public int TotalPages { get; private set; }

        private InfiniteScrollCollection<ProductModRequestViewModel> productModReqs = new InfiniteScrollCollection<ProductModRequestViewModel>();
        public InfiniteScrollCollection<ProductModRequestViewModel> ProductModReqs
        {
            get => productModReqs;
            set => SetProperty(ref productModReqs, value);
        }

        public Command RefreshCommand => new Command(
            async _ => 
            {
                ProductModReqs.Clear();
                TotalPages = 0;
                await ProductModReqs.LoadMoreAsync();
            });

        public Command<ProductModRequestViewModel> DeleteProductModReqCommand => new Command<ProductModRequestViewModel>(
            async pmr => 
            {
                try
                {
                    IsBusy = true;
                    ProductModRequestDTO model = pmr.GetModel();
                    await ProductModRequestService.DeleteAsync(model);
                    ProductModReqs.Remove(pmr);
                }
                catch (ServiceException ex)
                {
                    await App.CurrentPage.Err("Napaka pri brisanju", ex);
                }
                finally
                {
                    IsBusy = false;
                }
            });

        private IProductModRequestService ProductModRequestService => App.IoC.Resolve<IProductModRequestService>();

        public override async void OnPageAppearing()
        {
            base.OnPageAppearing();
            if (ProductModReqs.Count == 0)
            {
                await ProductModReqs.LoadMoreAsync();
            }
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
