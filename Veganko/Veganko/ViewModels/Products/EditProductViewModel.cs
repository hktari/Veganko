using Autofac;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Veganko.Common.Models.Products;
using Veganko.Extensions;
using Veganko.Models;
using Veganko.Services;
using Veganko.Services.Http;
using Veganko.Services.Http.Errors;
using Veganko.ViewModels.Products.ModRequests;
using Veganko.ViewModels.Products.ModRequests.Partial;
using Veganko.ViewModels.Products.Partial;
using Veganko.Views;
using Veganko.Views.Product.ModRequests;
using Xamarin.Forms;
namespace Veganko.ViewModels.Products
{
    public class EditProductViewModel : BaseEditProductViewModel
    {
        public const string ProductUpdatedMsg = "ProductUpdated";
        private readonly ProductViewModel originalProductVM;

        public EditProductViewModel(ProductViewModel product)
            : base(new ProductViewModel(product)) // Work on copy
        {
            this.originalProductVM = product;
        }

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
                    Product updatedProduct = CreateModel();
                    Product originalProduct = originalProductVM.MapToModel();
                    if (userService.CurrentUser.Role == Models.User.UserRole.Member)
                    {
                        ProductModRequestDTO editProductReq = new ProductModRequestDTO
                        {
                            UnapprovedProduct = updatedProduct,
                            ChangedFieldsAsList = GetChangedFields(originalProduct),
                            ExistingProductId = Product.Id,
                        };
                        editProductReq = await productModReqService.AddAsync(editProductReq);
                        if (HasImageBeenChanged)
                        {
                            editProductReq = await PostProductImages(editProductReq);
                        }

                        await App.CurrentPage.Inform("Produkt uspešno spremenjen. Takoj ko bo moderator potrdil vnešene informacije bodo spremembe vidne vsem !");

                        await App.Navigation.PopModalAsync();

                        ((MainPage)App.Current.MainPage)?.SetCurrentTab(2); // Profile page

                        // Navigate to product detail page from the ProductList page
                        ProductModRequestViewModel pmrVM = new ProductModRequestViewModel(editProductReq);
                        MessagingCenter.Send(this, BaseEditProductViewModel.ProductModReqAddedMsg, pmrVM);

                        await App.Navigation.PushAsync(
                            new ProductModRequestDetailPage(
                                new ProductModRequestDetailViewModel(
                                    pmrVM)));
                    }
                    else
                    {
                        updatedProduct = await productService.UpdateAsync(updatedProduct);
                        if (HasImageBeenChanged)
                        {
                            updatedProduct = await PostProductImages(updatedProduct);
                        }

                        Product.Update(updatedProduct);
                        Product.SetHasBeenSeen(true);

                        await App.Navigation.PopModalAsync();
                        MessagingCenter.Send(this, ProductUpdatedMsg, Product);
                    }
                }
                catch (ServiceConflictException<Product> sce)
                {
                    await HandleDuplicateError(sce);
                }
                catch (Exception ex)
                {
                    await App.CurrentPage.Err("Posodobitev produkta ni uspela.");
                    Logger.LogException(ex);
                }
                finally
                {
                    IsBusy = false;
                }
            });

        public Command CancelCommand => new Command(
            async () => await App.Navigation.PopModalAsync());
    }
}
