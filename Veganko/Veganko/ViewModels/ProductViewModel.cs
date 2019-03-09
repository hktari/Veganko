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
                    ShouldNotifyUIOnly = true;
                    SelectedProductClassifiers.Clear();
                    ShouldNotifyUIOnly = false;
                    UpdateSearchResults();
                }
            }
        }

        public List<Product> Products { get; private set; }

        #region TODO: make static
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
        #endregion

        private List<Product> matchesByText;
        private bool ShouldNotifyUIOnly { get; set; }

        public ProductViewModel()
        {
            Title = "Iskanje";
            Products = new List<Product>();
            SearchResult = new ObservableCollection<Product>();
            SelectedProductClassifiers = new ObservableCollection<ProductClassifier>();
            SelectedProductType = ProductType.NOT_SET;
            ShowProductClassifiers = true;

            LoadItemsCommand = new Command(async () => await RefreshProducts());
            
            MessagingCenter.Subscribe<NewProductPage, Product>(this, "AddItem", async (obj, item) =>
            {
                var _item = item as Product;
                if (await DataStore.AddItemAsync(_item))
                {
                    Products.Add(_item);
                    UnapplyFilters(false);
                }
            });
        }

        #region TODO: MOVE TO EXTENSIONS
        public IEnumerable<TEnum> GetValues<TEnum>()
        {
            var all = Enum.GetValues(typeof(TEnum));
            var tmp = new TEnum[all.Length];
            all.CopyTo(tmp, 0);
            return tmp;
        }
        #endregion

        public async Task RefreshProducts()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                SearchText = string.Empty;
                UnapplyFilters();
                Products = new List<Product>(
                    await DataStore.GetItemsAsync(true));
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

        public async Task DeleteProduct(Product product)
        {
            if (await DataStore.DeleteItemAsync(product))
            {
                Debug.Assert(Products != null);
                Products.Remove(product);
                SearchResult.Remove(product);
            }
            else
            {
                // TODO: Notify user
            }
        }

        private void UnapplyFilters(bool notifyUIOnly = true)
        {
            ShouldNotifyUIOnly = notifyUIOnly;
            SelectedProductType = ProductType.NOT_SET;
        }

        private void OnSelectedProductClassifierChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ShouldNotifyUIOnly)
            {
                ShouldNotifyUIOnly = false;
                return;
            }

            UpdateSearchResults();
        }

        private void UpdateSearchResults()
        {
            if (SelectedProductType == ProductType.NOT_SET)
            {
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    SetSearchResults(matchesByText);
                }
                else
                {
                    SetSearchResults(Products);
                }
            }
            else
            {
                List<Product> matches = null;
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    Debug.Assert(matchesByText != null);
                    matches = matchesByText.Where(
                        p =>
                        {
                            if (p.ProductClassifiers == null)
                                return false;
                            else if (p.Type != SelectedProductType)
                                return false;

                            if (SelectedProductClassifiers.Count > 0)
                                return p.ProductClassifiers.Intersect(SelectedProductClassifiers).Count() > 0;
                            else
                                return true;
                        }).ToList();
                }
                else
                {
                    matches = Products.Where(
                        p =>
                        {
                            if (p.ProductClassifiers == null)
                                return false;
                            else if (p.Type != SelectedProductType)
                                return false;

                            if (SelectedProductClassifiers.Count > 0)
                                return p.ProductClassifiers.Intersect(SelectedProductClassifiers).Count() > 0;
                            else
                                return true;
                        }).ToList();
                }

                SetSearchResults(matches);
            }
        }

        private void OnSearchClicked()
		{
            UnapplyFilters();

            matchesByText = Products
				.Where(p => p.Name.ToLower().Contains(SearchText.ToLower()) || p.Description.ToLower().Contains(SearchText.ToLower()))
                .ToList();

            SetSearchResults(matchesByText);
		}

        private async void OnBarcodeSearch()
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();

            var result = await scanner.Scan();

            if (result != null)
            {
                // TODO: disable application of filters ?
                UnapplyFilters();
                var query = Products.Where(p => p.Barcode == result.Text);
                SetSearchResults(query);
            }
        }

        private void OnSwitchFilteringOptions()
        {
            ShowProductClassifiers = !ShowProductClassifiers;
        }

        public void SetUserRights()
        {
            UserAccessRights = DependencyService.Get<IAccountService>().User.AccessRights;
        }

        private void SetSearchResults(IEnumerable<Product> items)
        {
            if (SearchResult == null)
                SearchResult = new ObservableCollection<Product>();

            SearchResult.Clear();
            foreach (var item in items)
                SearchResult.Add(item);
        }
    }
}
