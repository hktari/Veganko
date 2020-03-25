using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Veganko.Models;
using Veganko.Services;
using Xamarin.Forms;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Veganko.Common.Models.Products;

namespace Veganko.ViewModels
{
    public class FavoritesViewModel : BaseViewModel
    {
        public Command LoadItemsCommand => new Command(async () => await Refresh());

        private ObservableCollection<Product> items = new ObservableCollection<Product>();
        public ObservableCollection<Product> Items
        {
            get { return items; }
            set { SetProperty(ref items, value); }
        }

        public FavoritesViewModel()
        {
            
        }
        public async Task Refresh()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var favorites = await DependencyService.Get<IDataStore<Favorite>>().GetItemsAsync();
                var products = await DependencyService.Get<IDataStore<Product>>().GetItemsAsync();

                foreach (Favorite entry in favorites)
                {
                    var product = products.FirstOrDefault(p => p.Id == entry.ProductId);
                    if (product != null)
                        Items.Add(product);
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
