﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.ViewModels;
using Veganko.Views.Product;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManageProductsPage : BaseContentPage
    {
        ManageProductsViewModel vm;

        public ManageProductsPage()
        {
            InitializeComponent();
            BindingContext = vm = new ManageProductsViewModel();
        }

        protected override void OnAppearing()
        {
            vm.LoadItemsCommand.Execute(null);
        }

        async void OnProductSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Veganko.Models.Product;
            if (item == null)
                return;

            await Navigation.PushAsync(
                new ApproveProductPage(new ApproveProductViewModel(item)));

            // Manually deselect item.
            ProductsListView.SelectedItem = null;
        }

        async void OnDeleteProduct(object sender, EventArgs e)
        {
            string result = await DisplayActionSheet("Are you sure you wish to delete this product ?", "Cancel", "Yes");
            if (result == "Yes")
            {
                var mi = ((MenuItem)sender);
                await vm.DeleteProduct((Veganko.Models.Product)mi.CommandParameter);
            }
        }

        private void OnSearchTextInputCompleted(object sender, EventArgs e)
        {
            vm.SearchClickedCommand.Execute(null);
        }

    }
}