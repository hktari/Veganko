using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.ViewModels;
using Veganko.ViewModels.Products;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Veganko.Common.Models.Products;

namespace Veganko.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FavoritesPage : BaseContentPage
	{
        FavoritesViewModel vm;

		public FavoritesPage ()
		{
			InitializeComponent ();

            BindingContext = this.vm = new FavoritesViewModel();
        }
        protected async override void CustomOnAppearing()
        {
            await vm.Refresh().ConfigureAwait(false);
        }

        async void OnProductSelected(object sender, SelectedItemChangedEventArgs args)
        {
            //var item = args.SelectedItem as Product;
            //if (item == null)
            //    return;

            throw new NotImplementedException();
            //await Navigation.PushAsync(new ProductDetailPage(new ProductDetailViewModel(item)));

            // Manually deselect item.
            ProductsListView.SelectedItem = null;
        }
    }
}