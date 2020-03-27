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
using Xamarin.Forms;

namespace Veganko.ViewModels.Products.ModRequests
{
    public class ProductModRequestDetailViewModel : BaseViewModel
    {
        // TODO: mapping from model to vm
        // TODO: show / hide: store btn?, state, evaluators, time?
            // TODO: list on profile page

        public ProductModRequestDetailViewModel(ProductModRequestViewModel prodModReq)
        {
            ProdModReq = prodModReq;
            CanChangeState = UserService.CurrentUser.Role != Models.User.UserRole.Member;
            Product = new ProductViewModel(prodModReq.UnapprovedProduct);
        }

        public ProductModRequestViewModel ProdModReq { get; }
        public ProductViewModel Product { get; set; }
        public bool CanChangeState { get; }

        private IUserService UserService => App.IoC.Resolve<IUserService>();
        private IProductModRequestService ProductModRequestService => App.IoC.Resolve<IProductModRequestService>();

        public Command ApproveRequestCommand => new Command(
            async _ =>
            {
                try
                {
                    IsBusy = true;
                    //////await ProductModRequestService.Approve(ProdModReq);
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
                    //await ProductModRequestService.Reject(ProdModReq);
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
                    //await ProductModRequestService.DeleteAsync(ProdModReq);
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
    }
}
