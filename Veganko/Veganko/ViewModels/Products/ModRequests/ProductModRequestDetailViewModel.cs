using Autofac;
using System;
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
using Veganko.Views.Profile;
using Veganko.Views.Stores;
using Xamarin.Forms;

namespace Veganko.ViewModels.Products.ModRequests
{
    public class ProductModRequestDetailViewModel : BaseViewModel
    {
        public const string RemoveItemMsg = "RemoveItemMsg";
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

        /// <summary>
        /// Wheteher <see cref="EvaluatorsText"/> is empty.
        /// </summary>
        private bool anyEvaluators;
        public bool AnyEvaluators
        {
            get => anyEvaluators;
            set => SetProperty(ref anyEvaluators, value);
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
            LoadEvaluators();
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

                    MessagingCenter.Send(this, RemoveItemMsg, Product);
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

        public Command NavigateToAuthorProfileCommand => new Command(
            async () =>
            {
                if (Product.AuthorProfile != null)
                {
                    await App.Navigation.PushAsync(
                        new UserProfileDetailPage(
                            new Users.UserProfileDetailViewModel(Product.AuthorProfile)));
                }
            });

        public Command NavigateToEvaluatorProfileCommand => new Command(
            async () =>
            {
                if (Product.Model?.Evaluations?.FirstOrDefault()?.EvaluatorUserProfile is UserPublicInfo evaluator)
                {
                    await App.Navigation.PushAsync(
                        new UserProfileDetailPage(
                            new Users.UserProfileDetailViewModel(evaluator)));
                }
            });

        private string evaluatorsText;
        public string EvaluatorsText
        {
            get => evaluatorsText;
            set 
            {
                if (SetProperty(ref evaluatorsText, value))
                {
                    AnyEvaluators = !string.IsNullOrEmpty(value);
                }
            } 
        }

        public override async void OnPageAppearing()
        {
            base.OnPageAppearing();
            try
            {
                await Refresh();
            }
            catch (Exception ex)
            {
                Logger.LogException(new Exception("Failed to load detail data on appearing.", ex));
            }
        }

        public void LoadEvaluators()
        {
            if (Product.Model.Evaluations != null)
            {
                EvaluatorsText = string.Join(", ", Product.Model.Evaluations.Select(
                    eval => eval.EvaluatorUserProfile?.Username));
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
