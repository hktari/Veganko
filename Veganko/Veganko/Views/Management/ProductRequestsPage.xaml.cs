using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Extensions;
using Veganko.Services.Http;
using Veganko.ViewModels;
using Veganko.ViewModels.Management;
using Veganko.ViewModels.Products;
using Veganko.ViewModels.Products.Partial;
using Veganko.Views.Product;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views.Management
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProductRequestsPage : BaseContentPage
    {
        private readonly ProductRequestsViewModel vm;

        public ProductRequestsPage()
        {
            InitializeComponent();
            BindingContext = vm = new ProductRequestsViewModel();
        }

        void OnProductSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (args.SelectedItem == null)
                return;

            vm.ProductSelectedCommand.Execute(args.SelectedItem);
   
            //Manually deselect item.
           ProductsListView.SelectedItem = null;
        }

        void OnDeleteProduct(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            vm.DeleteProductModReqCommand.Execute(mi.CommandParameter);
        }
    }
}