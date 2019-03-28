using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views.Product
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ApproveProductPage : BaseContentPage
	{
        private ApproveProductViewModel vm;

        public ApproveProductPage (ApproveProductViewModel vm)
		{
			InitializeComponent ();
            BindingContext = this.vm = vm;
		}

        private async void OnApproveProductClicked(object sender, EventArgs arg)
        {
            var result = await DisplayActionSheet("Are you sure you wish to approve this product ?", "Cancel", "Yes");

            if (result == "Yes")
            {
                vm.ApproveProductCommand?.Execute(null);
            }
        }

        private async void OnDeleteProductClicked(object sender, EventArgs arg)
        {
            var result = await DisplayActionSheet("Are you sure you wish to delete this product ?", "Cancel", "Yes");

            if (result == "Yes")
            {
                vm.DeleteProductCommand?.Execute(null);
            }
        }
    }
}