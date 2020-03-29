using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Common.Models.Products;
using Veganko.Extensions;
using Veganko.Services;
using Veganko.Services.Http;
using Veganko.Services.Products.ProductModRequests;
using Veganko.ViewModels.Products.ModRequests.Partial;
using Veganko.ViewModels.Products.Partial;
using Veganko.ViewModels.Stores;
using Veganko.Views.Product;
using Veganko.Views.Stores;
using Xamarin.Forms;

namespace Veganko.ViewModels.Products.ModRequests
{
    public class ProductModRequestDetailViewModel : BaseViewModel
    {
        // TODO: show / hide: store btn?, time?
            // TODO: list on profile page

        public ProductModRequestDetailViewModel(ProductModRequestViewModel prodModReq)
        {
            ProdModReq = prodModReq;
            CanChangeState = UserService.CurrentUser.Role != Models.User.UserRole.Member;
            Product = new ProductViewModel(prodModReq.UnapprovedProduct);
            CanAddStores = prodModReq.Action == ProductModRequestAction.Add; // Edit requests can't add stores here coz productId is different.
            MessagingCenter.Unsubscribe<EditProductViewModel, ProductViewModel>(this, EditProductViewModel.ProductUpdatedMsg);
            MessagingCenter.Subscribe<EditProductViewModel, ProductViewModel>(this, EditProductViewModel.ProductUpdatedMsg, OnProductUpdated);
        }

        public ProductModRequestViewModel ProdModReq { get; }
        public ProductViewModel Product { get; }
        public bool CanChangeState { get; }
        public bool CanAddStores { get; }

        private IUserService UserService => App.IoC.Resolve<IUserService>();
        private IProductModRequestService ProductModRequestService => App.IoC.Resolve<IProductModRequestService>();

        public Command ApproveRequestCommand => new Command(
            async _ =>
            {
                try
                {
                    IsBusy = true;
                    ProductModRequestDTO model = ProdModReq.MapToModel();
                    model = await ProductModRequestService.ApproveAsync(model);
                    ProdModReq.Update(model);
                }
                catch (ServiceException ex)
                {
                    await App.CurrentPage.Err("Napaka pri potrjevanju.", ex);
                }
                finally
                {
                    IsBusy = false;
                }
            });

        public Command RejectRequestCommand => new Command(
            async _ =>
            {
                try
                {
                    IsBusy = true;
                    ProductModRequestDTO model = ProdModReq.MapToModel();
                    model = await ProductModRequestService.RejectAsync(model);
                    ProdModReq.Update(model);
                }
                catch (ServiceException ex)
                {
                    await App.CurrentPage.Err("Napaka pri zavrnitvi.", ex);
                }
                finally
                {
                    IsBusy = false;
                }
            });

        public Command DeleteRequestCommand => new Command(
            async _ => 
            {
                try
                {
                    IsBusy = true;
                    ProductModRequestDTO model = ProdModReq.MapToModel();
                    await ProductModRequestService.DeleteAsync(model);

                    // TODO: delete item from list
                    await App.Navigation.PopAsync();
                }
                catch (ServiceException ex)
                {
                    await App.CurrentPage.Err("Napaka pri brisanju.", ex);
                }
                finally
                {
                    IsBusy = false;

                }
            });

        public Command WhereToBuyCommand => new Command(
            async () => await App.Navigation.PushAsync(new StoreListPage(new StoreListViewModel(Product.Id))));

        public Command EditCommand => new Command(
               async () =>
               {
                   await App.Navigation.PushModalAsync(
                       new NavigationPage(
                           new EditProductPage(new EditProductViewModel(Product))));
               });

        private void OnProductUpdated(EditProductViewModel sender, ProductViewModel updatedProductViewModel)
        {
            Product.Update(updatedProductViewModel);
        }
    }
}
