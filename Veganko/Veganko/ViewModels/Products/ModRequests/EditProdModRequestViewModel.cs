using Autofac;
using System;
using Veganko.Common.Models.Products;
using Veganko.Extensions;
using Veganko.Services.Http.Errors;
using Veganko.Services.Products.ProductModRequests;
using Veganko.ViewModels.Products.ModRequests.Partial;
using Veganko.ViewModels.Products.Partial;
using Xamarin.Forms;

namespace Veganko.ViewModels.Products.ModRequests
{
    public class EditProdModRequestViewModel : BaseEditProductViewModel
    {
        public const string ProductModReqUpdatedMsg = "ProductModReqUpdated";

        private readonly ProductModRequestViewModel productModReq;

        public EditProdModRequestViewModel(ProductModRequestViewModel productModReq)
            : base(new ProductViewModel(productModReq.UnapprovedProduct))
        {
            this.productModReq = productModReq;
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
                   ProductModRequestDTO pmrModel = productModReq.MapToModel();
                   pmrModel.UnapprovedProduct = base.CreateModel();
                   pmrModel = await productModReqService.UpdateAsync(pmrModel);
                   if (HasImageBeenChanged)
                   {
                       pmrModel = await PostProductImages(pmrModel);
                   }

                   Product.Update(pmrModel.UnapprovedProduct);

                   await App.Navigation.PopModalAsync();
                   MessagingCenter.Send(this, ProductModReqUpdatedMsg, pmrModel);
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

        private IProductModRequestService ProductModRequestService => App.IoC.Resolve<IProductModRequestService>();
    }
}
