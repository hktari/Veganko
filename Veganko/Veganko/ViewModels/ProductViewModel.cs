using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Views;
using Xamarin.Forms;

namespace Veganko.ViewModels
{
	public class ProductViewModel : BaseViewModel
	{
        public Command LoadItemsCommand { get; set; }
        public Command SearchClickedCommand => new Command(OnSearchClicked);
        public Command SearchBarcodeCommand => new Command(OnBarcodeSearch);

        string searchText = "";
        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                SetProperty(ref searchText, value);
            }
        }

        private ObservableCollection<Product> searchResult;
        public ObservableCollection<Product> SearchResult {
            get
            {
                return searchResult;
            }
            set
            {
                SetProperty(ref searchResult, value);
            }
        }

        public ObservableCollection<Product> Products { get; private set; }

        public ProductViewModel()
        {
            Title = "Browse";
            Products = new ObservableCollection<Product>();
            SearchResult = new ObservableCollection<Product>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<NewProductPage, Product>(this, "AddItem", async (obj, item) =>
            {
                var _item = item as Product;
                Products.Add(_item);
                await DataStore.AddItemAsync(_item);
            });

            Products.CollectionChanged += OnProductsCollectionChanged;
        }

        private void OnProductsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach(var p in e.NewItems)
                        SearchResult.Add((Product)p); 
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var p in e.OldItems)
                        SearchResult.Remove((Product)p);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    searchResult.Clear();
                    break;
                default:
                    throw new NotImplementedException("Unhandled collection changed action !");
            }
        }

        void OnSearchClicked()
        {
            FilterProducts();
        }

        async void OnBarcodeSearch()
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();

            var result = await scanner.Scan();

            var query = Products.Where(p => p.Barcode == result.Text);
            ClearAndAddToSearchResult(query);
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

        private void FilterProducts()
        {
            var query = Products
                .Where(
                    p => p.Name.ToLower().Contains(SearchText.ToLower()) ||
                         p.Description.ToLower().Contains(SearchText.ToLower())
                );
            ClearAndAddToSearchResult(query);
        }

        private void ClearAndAddToSearchResult(IEnumerable<Product> items)
        {
            SearchResult.Clear();
            foreach (var item in items)
                SearchResult.Add(item);
        }
    }
}
