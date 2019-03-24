using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Veganko.Models;
using Veganko.Views;
using Xamarin.Forms;

namespace Veganko.ViewModels
{
    public class ManageProductsViewModel : ProductViewModel
    {
        public ManageProductsViewModel()
        {
            LoadItemsCommand = new Command(GetUnapprovedProducts);
        }

        private async void GetUnapprovedProducts()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                SearchText = string.Empty;
                UnapplyFilters();
                Products = new List<Product>(
                    await productService.GetUnapprovedAsync(true));
                SetSearchResults(Products);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
