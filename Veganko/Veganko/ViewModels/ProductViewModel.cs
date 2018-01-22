using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Views;
using Xamarin.Forms;

namespace Veganko.ViewModels
{
	public class ProductViewModel : BaseViewModel
	{
        public ObservableCollection<Product> Products { get; set; }
        public Command LoadItemsCommand { get; set; }

        public ProductViewModel()
        {
            Title = "Browse";
            Products = new ObservableCollection<Product>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<NewProductPage, Product>(this, "AddItem", async (obj, item) =>
            {
                var _item = item as Product;
                Products.Add(_item);
                await DataStore.AddItemAsync(_item);
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Products.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Products.Add(item);
                }
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
