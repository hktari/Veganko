using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using Veganko.Models;
using Veganko.Models.User;
using Veganko.Services;
using Xamarin.Forms;

namespace Veganko.ViewModels
{
    public class NewProductViewModel : BaseViewModel
    {
        public static List<string> PickerSource => new List<string>(Enum.GetNames(typeof(ProductType)));

        private Product product = new Product { Image = "raspeberry_meringue.jpg" };
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

        //public ObservableCollection<ProductClassifier> Selected
        //{
        //    get
        //    {
        //        return selected;
        //    }
        //    set
        //    {
        //        SetProperty(ref selected, value);
        //    }
        //}

        public Command PageAppeared => new Command(OnPageAppeared);

        public string Barcode
        {
            get
            {
                return Product.Barcode;
            }
            set
            {
                if (Product.Barcode == value)
                    return;
                Product.Barcode = value;
                OnPropertyChanged(nameof(Barcode));
            }
        }
        
        private void OnPageAppeared(object parameter)
        {
            var user = DependencyService.Get<IAccountService>().User;
            var mask = UserAccessRights.ProductsDelete;

            Debug.Assert(user != null);
            var hasApprovalRights = (user.AccessRights & mask) == mask;

            Product = new Product
            {
                Image = "raspeberry_meringue.jpg",
                //State = hasApprovalRights ? ProductState.Approved : ProductState.PendingApproval  // TODO: uncomment after testing
            };
        }
    }
}
