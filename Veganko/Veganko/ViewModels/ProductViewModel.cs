using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Controls;
using Veganko.Models;
using Veganko.Views;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Veganko.ViewModels
{
	public class ProductViewModel : BaseViewModel
	{
        public Command LoadItemsCommand { get; set; }
        public Command SearchClickedCommand => new Command(OnSearchClicked);
        public Command SearchBarcodeCommand => new Command(OnBarcodeSearch);
        public Command SwitchFilteringOptions => new Command(OnSwitchFilteringOptions);

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

        private bool showProductClassifiers = false;
        public bool ShowProductClassifiers
        {
            get
            {
                return showProductClassifiers;
            }
            set
            {
                SetProperty(ref showProductClassifiers, value);
            }
        }

        ObservableCollection<ProductClassifier> selectedProductClassifiers;
        public ObservableCollection<ProductClassifier> SelectedProductClassifiers
        {
            get
            {
                return selectedProductClassifiers;
            }
            set
            {
                SetProperty(ref selectedProductClassifiers, value);
            }
        }

        ObservableCollection<ProductType> selectedProductTypes;
        public ObservableCollection<ProductType> SelectedProductTypes
        {
            get
            {
                return selectedProductTypes;
            }
            set
            {
                SetProperty(ref selectedProductTypes, value);
            }
        }

        public ObservableCollection<Product> Products { get; private set; }

        public ObservableCollection<ProductClassifier> ProductClassifiers => new ObservableCollection<ProductClassifier>
        {
            ProductClassifier.Vegeterijansko,
            ProductClassifier.Vegansko,
            ProductClassifier.Pesketarijansko,
            ProductClassifier.GlutenFree,
            ProductClassifier.RawVegan,
            ProductClassifier.CrueltyFree
        };

        public ObservableCollection<ProductType> ProductTypes => new ObservableCollection<ProductType>
        {
            ProductType.Hrana,
            ProductType.Pijaca,
            ProductType.Kozmetika
        };

        public ProductViewModel()
        {
            Title = "Iskanje";
            Products = new ObservableCollection<Product>();
            SearchResult = new ObservableCollection<Product>();
            SelectedProductClassifiers = new ObservableCollection<ProductClassifier>()
            {
                ProductClassifier.NOT_SET,
                ProductClassifier.CrueltyFree,
                ProductClassifier.GlutenFree,
                ProductClassifier.Pesketarijansko,
                ProductClassifier.RawVegan,
                ProductClassifier.Vegansko,
                ProductClassifier.Vegeterijansko
            };
            SelectedProductClassifiers.CollectionChanged += OnSelectedProductClassifierChanged;

            SelectedProductTypes = new ObservableCollection<ProductType>()
            {
                ProductType.NOT_SET,
                ProductType.Hrana,
                ProductType.Pijaca,
                ProductType.Kozmetika
            };
            SelectedProductTypes.CollectionChanged += OnSelectedProductTypeChanged;

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<NewProductPage, Product>(this, "AddItem", async (obj, item) =>
            {
                var _item = item as Product;
                Products.Add(_item);
                await DataStore.AddItemAsync(_item);
            });

            Products.CollectionChanged += OnProductsCollectionChanged;
        }

        private void OnSelectedProductClassifierChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var collection = sender as ObservableCollection<ProductClassifier>;

            List<Product> matches;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ProductClassifier classifier in e.NewItems)
                    {
                        matches = Products.Where(p => p.ProductClassifiers.Contains(classifier)).ToList();
                        foreach(var product in matches)
                        {
                            if (!SearchResult.Contains(product))
                            {
                                SearchResult.Add(product);

                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (ProductClassifier classifier in e.OldItems)
                    {
                        matches = SearchResult.Where(p => p.ProductClassifiers.Contains(classifier)).ToList();
                        foreach (var product in matches)
                        {
                            if (!product.ProductClassifiers.Any(
                                p => collection.Contains(p)))
                            {
                                SearchResult.Remove(product);
                            }
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException("Unhandled collection changed action !");
            }
        }

        private void OnSelectedProductTypeChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var view = sender as SelectableEnumImageView<ProductClassifier>;

            List<Product> matches;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ProductType type in e.NewItems)
                    {
                        matches = Products.Where(p => p.Type == type).ToList();
                        foreach (var product in matches)
                        {
                            if (!SearchResult.Contains(product))
                            {
                                SearchResult.Add(product);

                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (ProductType type in e.OldItems)
                    {
                        matches = SearchResult.Where(p => p.Type == type).ToList();
                        foreach (var product in matches)
                            SearchResult.Remove(product);
                    }
                    break;
                default:
                    throw new NotImplementedException("Unhandled collection changed action !");
            }
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

            if (result != null)
            {
                var query = Products.Where(p => p.Barcode == result.Text);
                ClearAndAddToSearchResult(query);
            }
        }

        private void OnSwitchFilteringOptions()
        {
            ShowProductClassifiers = !ShowProductClassifiers;
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
