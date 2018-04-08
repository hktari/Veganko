using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
        protected override void OnAppearing()
        {
            base.OnAppearing();
            vm.Refresh();
        }
        async void OnProductSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Product;
            if (item == null)
                return;

            await Navigation.PushAsync(new ProductDetailPage(new ProductDetailViewModel(item)));

            // Manually deselect item.
            ProductsListView.SelectedItem = null;
        }
    }
}