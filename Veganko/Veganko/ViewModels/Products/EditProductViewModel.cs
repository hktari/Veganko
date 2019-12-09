using Autofac;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Veganko.Extensions;
using Veganko.Models;
using Veganko.Services;
using Veganko.Services.Http;
using Veganko.ViewModels.Products.Partial;
using Xamarin.Forms;

namespace Veganko.ViewModels.Products
{
    public class EditProductViewModel : BaseEditProductViewModel
    {
        public const string ProductUpdatedMsg = "ProductUpdated";

        private IProductService productService;

        public EditProductViewModel(ProductViewModel product)
            : base(new ProductViewModel(product)) // Work on copy
        {
            productService = App.IoC.Resolve<IProductService>();
        }

        public Command SaveCommand => new Command(
            async () =>
            {
                IsBusy = true;
                try
                {
                    Product updatedProduct = new Product();
                    Product.MapToModel(updatedProduct);
                    
                    updatedProduct = await productService.UpdateAsync(updatedProduct);

                    if (HasImageBeenChanged)
                    {
                        updatedProduct = await PostProductImages(updatedProduct);
                    }

                    Product.Update(updatedProduct);

                    await App.Navigation.PopModalAsync();
                    MessagingCenter.Send(this, ProductUpdatedMsg, Product);
                }
                catch (ServiceException ex)
                {
                    Debug.WriteLine(ex.Message + " " + ex.StackTrace);
                    await App.CurrentPage.Err("Posodobitev produkta ni uspela.");
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
