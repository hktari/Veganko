using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Other;
using Veganko.ViewModels.Products.ModRequests;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinImageUploader;
using Plugin.Media;

namespace Veganko.Views.Product.ModRequests
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProductModRequestDetailPage : BaseContentPage
	{
        private ProductModRequestDetailViewModel vm;

        public ProductModRequestDetailPage (ProductModRequestDetailViewModel vm)
		{
			InitializeComponent ();
            BindingContext = this.vm = vm;
        }

        //private async void OnApproveProductClicked(object sender, EventArgs arg)
        //{
        //    var result = await DisplayActionSheet("Are you sure you wish to approve this product ?", "Cancel", "Yes");

        //    if (result == "Yes")
        //    {
        //        await vm.ApproveProduct();
        //        await Navigation.PopAsync();
        //    }
        //}

        //private async void OnDeleteProductClicked(object sender, EventArgs arg)
        //{
        //    var result = await DisplayActionSheet("Are you sure you wish to delete this product ?", "Cancel", "Yes");

        //    if (result == "Yes")
        //    {
        //        await vm.DeleteProduct();
        //        await Navigation.PopAsync();
        //    }
        //}

        //protected override void CustomOnAppearing()
        //{
        //    vm.Init();
        //}
    }
}