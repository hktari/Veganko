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
using Veganko.Models.User;
using Veganko.Services;
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

        private UserAccessRights userAccessRights;
        public UserAccessRights UserAccessRights
        {
            get
            {
                return userAccessRights;
            }
            set
            {
                SetProperty(ref userAccessRights, value);
            }
        }

        string searchText = "";
        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                if (SetProperty(ref searchText, value))
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        matchesByText = null;
                    }
                }
            }
        }

        private ObservableCollection<Product> searchResult;
        public ObservableCollection<Product> SearchResult
        {
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
                if (selectedProductClassifiers != null)
                {
                    selectedProductClassifiers.CollectionChanged -= OnSelectedProductClassifierChanged;
                }

                if (SetProperty(ref selectedProductClassifiers, value) && value != null)
                {
                    OnSelectedProductClassifierChanged(value, 
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
                    value.CollectionChanged += OnSelectedProductClassifierChanged;
                }
            }
        }

        //ObservableCollection<ProductType> selectedProductTypes;
        //public ObservableCollection<ProductType> SelectedProductTypes
        //{
        //    get
        //    {
        //        return selectedProductTypes;
        //    }
        //    set
        //    {
        //        if (selectedProductTypes != null)
        //        {
        //            selectedProductTypes.CollectionChanged -= OnSelectedProductTypeChanged;
        //        }

        //        if (SetProperty(ref selectedProductTypes, value) && value != null)
        //        {
        //            OnSelectedProductTypeChanged(value,
        //                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
        //            value.CollectionChanged += OnSelectedProductTypeChanged;
        //        }
        //    }
        //}
        private bool defaultClassifiersUpdated = false;

        private ProductType selectedProductType;
        public ProductType SelectedProductType
        {
            get
            {
                return selectedProductType;
            }
            set
            {
                if (SetProperty(ref selectedProductType, value))
                {
                    SelectedProductClassifiers.Clear();
                }
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

        private List<Product> matchesByText;
        private List<Product> matchesByClassifiers;

        public ProductViewModel()
        {
            Title = "Iskanje";
            Products = new ObservableCollection<Product>();
            SearchResult = new ObservableCollection<Product>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            SelectedProductClassifiers = new ObservableCollection<ProductClassifier>();
            SelectedProductType = ProductType.NOT_SET;

            MessagingCenter.Subscribe<NewProductPage, Product>(this, "AddItem", async (obj, item) =>
            {
                var _item = item as Product;
                if (await DataStore.AddItemAsync(_item))
                {
                    Products.Add(_item);
                    SearchResult.Insert(0, _item);
                    UnapplyFilters();
                }
            });
        }

        public void UnapplyFilters(bool notifyUIOnly = true)
        {
            this.notifyUIOnly = notifyUIOnly;

            SelectedProductType = ProductType.NOT_SET;
            SelectedProductClassifiers?.Clear();
            // Do this only if if wasn't already done by setting the property above
            //if (!defaultClassifiersUpdated)
            //    SelectDefaultClassifiers();

            // Reset
            //defaultClassifiersUpdated = false;
        }

        public IEnumerable<TEnum> GetValues<TEnum>()
        {
            var all = Enum.GetValues(typeof(TEnum));
            var tmp = new TEnum[all.Length];
            all.CopyTo(tmp, 0);
            return tmp;
        }

        public async Task DeleteProduct(Product product)
        {
            if (await DataStore.DeleteItemAsync(product))
            {
                await ExecuteLoadItemsCommand();
                UnapplyFilters();
            }
            else
            {
                // TODO: Notify user
            }
        }

        private void OnSelectedProductClassifierChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (notifyUIOnly)
            {
                matchesByClassifiers = null;
                notifyUIOnly = false;
                return;
            }

            var classifiers = sender as ObservableCollection<ProductClassifier>;
            var newClassifiers = (IList<ProductClassifier>)e.NewItems;

            List<Product> matches;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    matches = Products.Where(
                        p =>
                        {
                            if (p.ProductClassifiers == null)
                                return false;

                            return p.ProductClassifiers.Union(newClassifiers).Count() > 0;
                        }).ToList();
                    {
                        
                        //foreach(var product in matches)
                        //{
                        //    if (!SearchResult.Contains(product))
                        //    {
                        //        SearchResult.Add(product);
                        //    }
                        //}
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    matches = SearchResult.Where(
                        p =>
                        {
                            Debug.Assert(p.ProductClassifiers != null);
                            return p.ProductClassifiers.Union(classifiers).Count() > 0;
                        }).ToList();
                    break;
                case NotifyCollectionChangedAction.Reset:
                    matches = new List<Product>();
                    break;
                default:
                    throw new NotImplementedException("Unhandled collection changed action !");
            }

            matchesByClassifiers = matches;

            UpdateSearchResults();
        }

        private void UpdateSearchResults()
        {
            // TODO: IF TEXT SEARCH IN TEXT RESULTS BY PRODUCT TYPE / CLASSIFIER
            //      ELSE SEARCH IN PRODUCTS BY PRODUCT TYPE / CLASSIFIER
            IEnumerable<Product> finalMatches = null;

            if (SelectedProductType != ProductType.NOT_SET)
            {
                finalMatches = Products.Where(p => p.Type == SelectedProductType);

                if (matchesByClassifiers != null)
                {
                    finalMatches = finalMatches.Union(matchesByClassifiers);
                }

                if (matchesByText != null)
                {
                    finalMatches = finalMatches.Union(matchesByText);
                }
            }

            SetSearchResults(finalMatches ?? Products);
        }

        private bool notifyUIOnly = false;

        void OnSearchClicked()
		{
			matchesByText = Products
				.Where(p => p.Name.ToLower().Contains(SearchText.ToLower()) || p.Description.ToLower().Contains(SearchText.ToLower()))
                .ToList();

            UnapplyFilters();
            SetSearchResults(matchesByText);
		}

        async void OnBarcodeSearch()
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();

            var result = await scanner.Scan();

            if (result != null)
            {
                var query = Products.Where(p => p.Barcode == result.Text);
                // TODO: disable application of filters ?
                UnapplyFilters();
                SetSearchResults(query);
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
                UserAccessRights = DependencyService.Get<IAccountService>().User.AccessRights;

                Products.Clear();
                var items = await DataStore.GetItemsAsync(true);
                Products = new ObservableCollection<Product>(items);
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

        private void SetSearchResults(IEnumerable<Product> items)
        {
            SearchResult.Clear();
            foreach (var item in items)
                SearchResult.Add(item);
        }
    }
}
