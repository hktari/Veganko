using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Common.Models.Products;
using Veganko.Common.Models.Users;
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
        public const string DeletedMsg = "DeletedMsg";

        public ProductModRequestDetailViewModel(ProductModRequestViewModel prodModReq)
        {
            Product = prodModReq;
            CanChangeState = UserService.CurrentUser.Role != UserRole.Member;
            CanAddStores = prodModReq.Model.Action == ProductModRequestAction.Add; // Edit requests can't add stores here coz productId is different.
            MessagingCenter.Unsubscribe<EditProdModRequestViewModel, ProductModRequestDTO>(this, EditProdModRequestViewModel.ProductModReqUpdatedMsg);
            MessagingCenter.Subscribe<EditProdModRequestViewModel, ProductModRequestDTO>(this, EditProdModRequestViewModel.ProductModReqUpdatedMsg, OnProductUpdated);
        }

        public ProductModRequestViewModel Product { get; }
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
                    ProductModRequestDTO model = Product.GetModel();
                    model = await ProductModRequestService.ApproveAsync(model);
                    Product.Update(model);
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
                    ProductModRequestDTO model = Product.GetModel();
                    model = await ProductModRequestService.RejectAsync(model);
                    Product.Update(model);
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
                    ProductModRequestDTO model = Product.GetModel();
                    await ProductModRequestService.DeleteAsync(model);

                    MessagingCenter.Send(this, DeletedMsg, Product);
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
                           new EditProductPage(new EditProdModRequestViewModel(Product))));
               });

        private void OnProductUpdated(EditProdModRequestViewModel sender, ProductModRequestDTO updatedPMR)
        {
            Product.Update(updatedPMR);
        }
    }
}
