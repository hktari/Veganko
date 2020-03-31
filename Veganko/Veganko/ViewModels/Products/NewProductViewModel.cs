using Autofac;
using System;
using System.Collections.ObjectModel;
using Veganko.Common.Models.Products;
using Veganko.Extensions;
using Veganko.Models.User;
using Veganko.Services.DB;
using Veganko.Services.Http.Errors;
using Veganko.ViewModels.Products.ModRequests;
using Veganko.ViewModels.Products.ModRequests.Partial;
using Veganko.ViewModels.Products.Partial;
using Veganko.Views;
using Veganko.Views.Product.ModRequests;
using Xamarin.Forms;

namespace Veganko.ViewModels.Products
{
    public class NewProductViewModel : BaseEditProductViewModel
    {
        public const string ProductAddedMsg = "ProductAdded";

        public Command SaveCommand => new Command(
            async () =>
            {
                if (!ValidateFields())
                {
                    return;
                }

                try
                {
                    IsBusy = true;

                    Product productModel = CreateModel();

                    if (userService.CurrentUser.Role == UserRole.Member)
                    {
                        ProductModRequestDTO addProdRequest = new ProductModRequestDTO
                        {
                            UnapprovedProduct = productModel,
                        };
                        addProdRequest = await productModReqService.AddAsync(addProdRequest);
                        addProdRequest = await PostProductImages(addProdRequest);
                        productModel = addProdRequest.UnapprovedProduct;

                        await App.CurrentPage.Inform("Produkt uspešno dodan. Takoj ko bo moderator potrdil, da so vnešene informacije v redu, bo produkt viden vsem !");

                        ((MainPage)App.Current.MainPage)?.SetCurrentTab(2); // Profile page
                        
                        // Navigate to product detail page from the ProductList page
                        ProductModRequestViewModel pmrVM = new ProductModRequestViewModel(addProdRequest);
                        MessagingCenter.Send(this, ProductAddedMsg, pmrVM);

                        await App.Navigation.PushAsync(
                            new ProductModRequestDetailPage(
                                new ProductModRequestDetailViewModel(
                                    pmrVM)));
                    }
                    else
                    {
                        productModel = await productService.AddAsync(productModel);
                        productModel = await PostProductImages(productModel);

                        Product.Update(productModel);

                        // Mark the product as seen
                        Product.SetHasBeenSeen(true);
                        await App.IoC.Resolve<IProductDBService>().SetProductsAsSeen(new[] { productModel });

                        ((MainPage)App.Current.MainPage)?.SetCurrentTab(0);
                        MessagingCenter.Send(this, ProductAddedMsg, Product);

                        // Navigate to product detail page from the ProductList page
                        await App.Navigation.PushAsync(
                            new ProductDetailPage(
                                new ProductDetailViewModel(Product)));
                    }

                    // Mark product to be initialized the next the page appears.
                    Product = null;
                }
                catch (ServiceConflictException<Product> sce)
                {
                    await HandleDuplicateError(sce);
                }
                catch (Exception ex)
                {
                    await App.CurrentPage.Err("Napak pri dodajanju");
                    Logger.LogException(ex);
                }
                finally
                {
                    IsBusy = false;
                }
            });

        public Command ScrollToTopCommand { get; set; }

        public override void OnPageAppearing()
        {
            base.OnPageAppearing();

            if (Product == null)
            {
                InitProduct();
                ScrollToTopCommand?.Execute(null);
            }
        }

        private void InitProduct()
        {
            // TODO: When implementing member product adding functionality
            //var user = App.IoC.Resolve<IUserService>().CurrentUser;
            //var mask = UserAccessRights.ProductsDelete;

            //Debug.Assert(user != null);
            //var hasApprovalRights = (user.Role.ToUAC() & mask) == mask;

            Product = new ProductViewModel
            {
                ProductClassifiers = new ObservableCollection<ProductClassifier>(),
            };
            SelectedProductType = ProductType.Hrana;
            Barcode = null;
            PhotoPicked = false;
            BarcodePicked = false;
        }
    }
}
