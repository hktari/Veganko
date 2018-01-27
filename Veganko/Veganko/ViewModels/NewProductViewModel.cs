using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Models;
using Xamarin.Forms;

namespace Veganko.ViewModels
{
    public class NewProductViewModel : BaseViewModel
    {
        public Product Product { get; set; }
        
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

        public NewProductViewModel()
        {
            Product = new Product
            {
                Image = "raspeberry_meringue.jpg"
            };
        }
    }
}
