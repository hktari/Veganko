using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Models;
using Veganko.Services;
using Xamarin.Forms;

namespace Veganko.ViewModels
{
    public class ApproveProductViewModel : BaseViewModel
    {
        public Product Product { get; set; }

        public Command ApproveProductCommand { get; }

        public Command DeleteProductCommand { get; }

        private readonly IProductService productService;

        public ApproveProductViewModel(Product product)
        {
            Product = product;
            ApproveProductCommand = new Command(ApproveProduct);
            DeleteProductCommand = new Command(DeleteProduct);
            productService = DependencyService.Get<IProductService>();
        }

        private void DeleteProduct()
        {
            productService.DeleteAsync(Product);
        }

        private void ApproveProduct()
        {
            Product.State = ProductState.Approved;
            productService.UpdateAsync(Product);
        }
    }
}
