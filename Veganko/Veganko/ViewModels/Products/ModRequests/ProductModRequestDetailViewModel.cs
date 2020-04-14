using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Veganko.Common.Models.Products;
using Veganko.Common.Models.Users;
using Veganko.Extensions;
using Veganko.Other;
using Veganko.Services;
using Veganko.Services.Http;
using Veganko.Services.Http.Errors;
using Veganko.Services.Logging;
using Veganko.Services.Products.ProductModRequests;
using Veganko.ViewModels.Products.ModRequests.Partial;
using Veganko.ViewModels.Stores;
using Veganko.Views;
using Veganko.Views.Product;
using Veganko.Views.Stores;
using Xamarin.Forms;

namespace Veganko.ViewModels.Products.ModRequests
{
    public class ProductModRequestDetailViewModel : BaseViewModel
    {
        public const string ItemRemovedMsg = "ItemRemovedMsg";
        private const string RequestHandledErrMsg = "Prošnja je že bila obravnavana.";

        public ProductModRequestDetailViewModel(ProductModRequestViewModel prodModReq)
        {
            Product = prodModReq;
            OnProductUpdated();
            CanAddStores = prodModReq.Model.Action == ProductModRequestAction.Add; // Edit requests can't add stores here coz productId is different.
            MessagingCenter.Unsubscribe<EditProdModRequestViewModel, ProductModRequestDTO>(this, EditProdModRequestViewModel.ProductModReqUpdatedMsg);
            MessagingCenter.Subscribe<EditProdModRequestViewModel, ProductModRequestDTO>(this, EditProdModRequestViewModel.ProductModReqUpdatedMsg, OnProductUpdated);
        }

        public ProductModRequestViewModel Product { get; }

        private bool canChangeState;
        public bool CanChangeState
        {
            get => canChangeState;
            set => SetProperty(ref canChangeState, value);
        }

        public bool CanAddStores { get; }

        private bool canEditProduct;
        public bool CanEditProduct
        {
            get => canEditProduct;
            set => SetProperty(ref canEditProduct, value);
        }

        private IUserService UserService => App.IoC.Resolve<IUserService>();
        private IProductModRequestService ProductModRequestService => App.IoC.Resolve<IProductModRequestService>();
        private ILogger Logger => App.IoC.Resolve<ILogger>();

        public Command ApproveRequestCommand => new Command(
            async _ =>
            {
                try
                {
                    IsBusy = true;
                    ProductModRequestDTO model = Product.GetModel();
                    model = await ProductModRequestService.ApproveAsync(model);
                    Product.Update(model);
                    OnProductUpdated();

                    await App.Navigation.PopAsync();
                }
                catch (ServiceConflictException<Product> conflictEx)
                {
                    await App.CurrentPage.Err("Produkt s to črtno kodo že obstaja.");
                    await App.Navigation.PushAsync(
                        new ProductDetailPage(
                            new ProductDetailViewModel(conflictEx.RequestConflict.ConflictingItem)));
                }
                catch (ServiceException ex)
                {
                    string errMsg = "Napaka pri potrjevanju.";
                    if (ex.ErrorCode != null && (ProdModReqErrorCode)ex.ErrorCode == ProdModReqErrorCode.AlreadyHandled)
                    {
                        errMsg = RequestHandledErrMsg;
                        await Refresh();
                    }

                    await App.CurrentPage.Err(errMsg, ex);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    await App.CurrentPage.Err(Strings.UnknownErr);
                }
                finally
                {
                    IsBusy = false;
                }
            });

        private async Task Refresh()
        {
            var productUpdate = await ProductModRequestService.GetAsync(Product.Model.Id);
            Product.Update(productUpdate);
            OnProductUpdated();
            await LoadEvaluators();
        }

        public Command RejectRequestCommand => new Command(
            async _ =>
            {
                try
                {
                    IsBusy = true;
                    ProductModRequestDTO model = Product.GetModel();
                    model = await ProductModRequestService.RejectAsync(model);
                    Product.Update(model);
                    OnProductUpdated();
                    await App.Navigation.PopAsync();
                }
                catch (ServiceException ex)
                {
                    string errMsg = "Napaka pri zavrnitvi.";
                    if (ex.ErrorCode != null && (ProdModReqErrorCode)ex.ErrorCode == ProdModReqErrorCode.AlreadyHandled)
                    {
                        errMsg = RequestHandledErrMsg;
                        await Refresh();
                    }

                    await App.CurrentPage.Err(errMsg, ex);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    await App.CurrentPage.Err(Strings.UnknownErr);
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

                    MessagingCenter.Send(this, ItemRemovedMsg, Product);
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

        private string evaluatorsText;
        public string EvaluatorsText
        {
            get => evaluatorsText;
            set => SetProperty(ref evaluatorsText, value);
        }

        public override async void OnPageAppearing()
        {
            base.OnPageAppearing();
            await LoadEvaluators();
        }

        public async Task LoadEvaluators()
        {
            try
            {
                var evaluators = new List<UserPublicInfo>();
                if (Product.Model.Evaluations != null)
                {
                    foreach (var evaluation in Product.Model.Evaluations)
                    {
                        evaluators.Add(
                            await UserService.Get(evaluation.EvaluatorUserId));
                    }
                }

                EvaluatorsText = string.Join(", ", evaluators.Select(eval => eval.Username));
            }
            catch (ServiceException ex)
            {
                Logger.LogException(ex);
                EvaluatorsText = "napaka pri nalaganju.";
            }
        }

        private void OnProductUpdated()
        {
            CanChangeState = UserService.CurrentUser.Role != UserRole.Member && Product.State == ProductModRequestState.Pending;
            CanEditProduct = Product.State == ProductModRequestState.Pending;
        }

        private void OnProductUpdated(EditProdModRequestViewModel sender, ProductModRequestDTO updatedPMR)
        {
            Product.Update(updatedPMR);
        }
    }
}
