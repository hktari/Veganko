using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.ViewModels;
using Veganko.ViewModels.Products;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views.Product
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditProductPage : ContentPage
    {
        private readonly EditProductViewModel vm;

        public EditProductPage(EditProductViewModel vm)
        {
            InitializeComponent();
            BindingContext = this.vm = vm;
        }
    }
}