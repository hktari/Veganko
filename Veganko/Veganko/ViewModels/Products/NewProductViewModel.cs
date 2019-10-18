using Autofac;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Veganko.Extensions;
using Veganko.Models;
using Veganko.Models.User;
using Veganko.Other;
using Veganko.Services;
using Veganko.Services.Http;
using Veganko.Views;
using Xamarin.Forms;

namespace Veganko.ViewModels.Products
{
    public class NewProductViewModel : BaseEditProductViewModel
    {
        public const string ProductAddedMsg = "ProductAdded";

        private IProductService productService;

        public NewProductViewModel()
        {
            productService = App.IoC.Resolve<IProductService>();
        }

        public Command PageAppeared => new Command(OnPageAppeared);

        public Command SaveCommand => new Command(
            async () =>
            {
                try
                {
                    Product = await productService.AddAsync(Product);
                    ((MainPage)App.Current.MainPage).SetCurrentTab(0);
                    MessagingCenter.Send(this, ProductAddedMsg, Product);
                }
                catch (ServiceException ex)
                {
                    await App.CurrentPage.Err("Napak pri dodajanju: " + ex.Response);
                }
            });

        private void OnPageAppeared(object parameter)
        {
            InitProduct();
        }

        private void InitProduct()
        {
            var user = App.IoC.Resolve<IUserService>().CurrentUser;
            var mask = UserAccessRights.ProductsDelete;

            Debug.Assert(user != null);
            // TODO:
            var hasApprovalRights = (user.Role.ToUAC() & mask) == mask;

            Product = new Models.Product
            {
                //State = hasApprovalRights ? ProductState.Approved : ProductState.PendingApproval  // TODO: uncomment after testing
                ProductClassifiers = new ObservableCollection<ProductClassifier>(),
            };
            SelectedProductType = (ProductType)0;
            Barcode = null;
            PhotoPicked = false;
            BarcodePicked = false;
        }
    }
}
