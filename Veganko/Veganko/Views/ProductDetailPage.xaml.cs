using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProductDetailPage : BaseContentPage
	{
        ProductDetailViewModel vm;
		public ProductDetailPage (ProductDetailViewModel vm)
		{
			InitializeComponent ();
            BindingContext = this.vm = vm;
		}
        public ProductDetailPage() : this(null) { } // Satisfy the Xamarin.Forms previewer

        protected override void OnAppearing()
        {
            base.OnAppearing();
            vm.RefreshComments();
        }
    }
}