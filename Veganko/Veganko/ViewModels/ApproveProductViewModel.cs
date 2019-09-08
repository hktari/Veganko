using Autofac;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Other;
using Veganko.Services;
using Xamarin.Forms;

namespace Veganko.ViewModels
{
    public class ApproveProductViewModel : BaseViewModel
    {
        // To control initialization
        private Product tmpProduct;

        private Product product;
        public Product Product
        {
            get => product;
            set => SetProperty(ref product, value);
        }

        private ProductType selectedProductType;
        public ProductType SelectedProductType
        {
            get
            {
                return selectedProductType;
            }
            set
            {
                if (SetProperty(ref selectedProductType, value))
                {
                    ProductClassifiers?.Clear();
                    ProductClassifiers = new ObservableCollection<ProductClassifier>(EnumConfiguration.ClassifierDictionary[value]);
                }
            }
        }

        private ObservableCollection<ProductClassifier> productClassifiers;
        public ObservableCollection<ProductClassifier> ProductClassifiers
        {
            get => productClassifiers;
            set => SetProperty(ref productClassifiers, value);
        }

        private readonly IProductService productService;

        public ApproveProductViewModel(Product product)
        {
            productService = App.IoC.Resolve<IProductService>();
            tmpProduct = product;
        }

        public void Init()
        {
            SelectedProductType = tmpProduct.Type;
            Product = tmpProduct;
            tmpProduct = null;
        }

        public Task DeleteProduct()
        {
            return productService.DeleteAsync(Product);
        }

        public Task ApproveProduct()
        {
            Product.Type = selectedProductType;
            Product.State = ProductState.Approved;
            return productService.UpdateAsync(Product);
        }
    }
}
