using Autofac;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using Veganko.Models;
using Veganko.Models.User;
using Veganko.Other;
using Veganko.Services;
using Xamarin.Forms;

namespace Veganko.ViewModels
{
    public class NewProductViewModel : BaseViewModel
    {
        private Product product;
        public Product Product
        {
            get
            {
                return product;
            }
            set
            {
                SetProperty(ref product, value);
            }
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
                    Product.Type = selectedProductType;
                }
            }
        }

        private ObservableCollection<ProductClassifier> productClassifiers;

        public ObservableCollection<ProductClassifier> ProductClassifiers
        {
            get => productClassifiers;
            set => SetProperty(ref productClassifiers, value);
        }

        public Command PageAppeared => new Command(OnPageAppeared);

        public string Barcode
        {
            get
            {
                return Product?.Barcode;
            }
            set
            {
                if (Product?.Barcode == value)
                    return;
                Product.Barcode = value;
                OnPropertyChanged(nameof(Barcode));
            }
        }

        private void OnPageAppeared(object parameter)
        {
            InitProduct();
        }

        private void InitProduct()
        {
            var user = App.IoC.Resolve<IAccountService>().User;
            var mask = UserAccessRights.ProductsDelete;

            Debug.Assert(user != null);
            var hasApprovalRights = (user.AccessRights & mask) == mask;

            Product = new Product
            {
                Image = "raspeberry_meringue.jpg",
                //State = hasApprovalRights ? ProductState.Approved : ProductState.PendingApproval  // TODO: uncomment after testing
                ProductClassifiers = new ObservableCollection<ProductClassifier>(),
            };
            SelectedProductType = (ProductType)1;
            Barcode = null;
        }
    }
}
