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
using Veganko.Services.DB;
using Veganko.Services.Http;
using Veganko.ViewModels.Products.Partial;
using Veganko.Views;
using Xamarin.Forms;

namespace Veganko.ViewModels.Products
{
    public class NewProductViewModel : BaseEditProductViewModel
    {
        public const string ProductAddedMsg = "ProductAdded";

        public Command SaveCommand => new Command(
            async () =>
            {
                if (!HasImageBeenChanged)
                {
                    await App.CurrentPage.Err("Prosim dodaj sliko");
                    return;
                }

                try
                {
                    IsBusy = true;

                    Product productModel = new Product();
                    Product.MapToModel(productModel);

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

                    // Mark product to be initialized the next the page appears.
                    Product = null;
                }
                catch (ServiceException ex)
                {
                    await App.CurrentPage.Err("Napak pri dodajanju: " + ex.Response);
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
