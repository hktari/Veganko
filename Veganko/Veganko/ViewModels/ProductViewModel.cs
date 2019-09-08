using Autofac;
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
using Veganko.Other;
using Veganko.Services;
using Veganko.Services.Http;
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

        private UserRole userRole;
        public UserRole UserRole
        {
            get
            {
                return userRole;
            }
            set
            {
                SetProperty(ref userRole, value);
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
                    ProductClassifiers = new ObservableCollection<ProductClassifier>(EnumConfiguration.ClassifierDictionary[value]);
                    ShouldNotifyUIOnly = false;
                    UpdateSearchResults();
                }
            }
        }

        public List<Product> Products { get; protected set; }

        #region TODO: make static
        private ObservableCollection<ProductClassifier> productClassifiers;

        public ObservableCollection<ProductClassifier> ProductClassifiers
        {
            get => productClassifiers;
            set => SetProperty(ref productClassifiers, value);
        }

        public ObservableCollection<ProductType> ProductTypes => new ObservableCollection<ProductType>
        {
            ProductType.Hrana,
            ProductType.Pijaca,
            ProductType.Kozmetika
        };
        #endregion

        private List<Product> matchesByText;
        private bool ShouldNotifyUIOnly { get; set; }
        protected readonly IProductService productService;
        protected readonly IAccountService accountService;

        public ProductViewModel()
        {
            productService = App.IoC.Resolve<IProductService>();
            accountService = App.IoC.Resolve<IAccountService>();

            Title = "Iskanje";
            Products = new List<Product>();
            SearchResult = new ObservableCollection<Product>();
            SelectedProductClassifiers = new ObservableCollection<ProductClassifier>();
            SelectedProductType = ProductType.NOT_SET;
            ShowProductClassifiers = true;
            UserRole = accountService.User.Role;

            LoadItemsCommand = new Command(async () => await RefreshProducts());
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
                // TODO: handle pages
                Products = new List<Product>(
                    (await productService.AllAsync(forceRefresh: true)).Items);
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
            await productService.DeleteAsync(product);
            Debug.Assert(Products != null);
            Products.Remove(product);
            SearchResult.Remove(product);
        }

        protected void SetSearchResults(IEnumerable<Product> items)
        {
            if (SearchResult == null)
                SearchResult = new ObservableCollection<Product>();

            SearchResult.Clear();
            foreach (var item in items)
                SearchResult.Add(item);
        }

        protected void UnapplyFilters(bool notifyUIOnly = true)
        {
            ShouldNotifyUIOnly = notifyUIOnly;
            SelectedProductType = ProductType.NOT_SET;
        }

        protected void UpdateSearchResults()
        {
            if (SelectedProductType == ProductType.NOT_SET)
            {
                if (matchesByText != null)
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

        private void OnSelectedProductClassifierChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ShouldNotifyUIOnly)
            {
                ShouldNotifyUIOnly = false;
                return;
            }

            UpdateSearchResults();
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

        public async Task AddNewProduct(Product product)
        {
            product = await productService.AddAsync(product);
            Products.Add(product);
            UnapplyFilters(false);
        }

        private void OnSwitchFilteringOptions()
        {
            ShowProductClassifiers = !ShowProductClassifiers;
        }
    }
}
