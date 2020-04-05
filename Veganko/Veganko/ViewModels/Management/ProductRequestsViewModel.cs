using Autofac;
using System.Linq;
using Veganko.Common.Models.Products;
using Veganko.Extensions;
using Veganko.Models;
using Veganko.Services.Http;
using Veganko.Services.Products.ProductModRequests;
using Veganko.ViewModels.Products.ModRequests.Partial;
using Xamarin.Forms;
using Xamarin.Forms.Extended;

namespace Veganko.ViewModels.Management
{
    public class ProductRequestsViewModel : BaseViewModel
    {
        private const int PageSize = 2;

        public ProductRequestsViewModel()
        {
            ResetCollection();
            ProductModReqs = new InfiniteScrollCollection<ProductModRequestViewModel>()
            {
                OnCanLoadMore = () => CurrentPage + 1 <= TotalPages,
                OnLoadMore = async () =>
                {
                    try
                    {
                        IsBusy = true;
                        // load the next page
                        var nextPage = ProductModReqs.Count == 0 ? 1 : CurrentPage + 1;

                        PagedList<ProductModRequestDTO> page = await ProductModRequestService.AllAsync(nextPage, PageSize, state: ProductModRequestState.Pending);
                        TotalPages = page.TotalPages;

                        AnyItems = page.Items.Count() > 0;

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

        public int CurrentPage => (ProductModReqs.Count / TotalPages) + 1;
        public int TotalPages { get; private set; }

        private bool anyItems;
        public bool AnyItems
        {
            get => anyItems;
            set => SetProperty(ref anyItems, value);
        }

        private InfiniteScrollCollection<ProductModRequestViewModel> productModReqs = new InfiniteScrollCollection<ProductModRequestViewModel>();
        public InfiniteScrollCollection<ProductModRequestViewModel> ProductModReqs
        {
            get => productModReqs;
            set => SetProperty(ref productModReqs, value);
        }

        private void ResetCollection()
        {
            ProductModReqs.Clear();
            TotalPages = 0;
        }

        public Command RefreshCommand => new Command(
            async _ =>
            {
                ResetCollection();
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
