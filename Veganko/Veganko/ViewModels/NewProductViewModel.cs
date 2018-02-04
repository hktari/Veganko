using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Veganko.Models;
using Xamarin.Forms;

namespace Veganko.ViewModels
{
    public class NewProductViewModel : BaseViewModel
    {
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
            Product = new Product
            {
                Image = "raspeberry_meringue.jpg"
            };
        }
    }
}
