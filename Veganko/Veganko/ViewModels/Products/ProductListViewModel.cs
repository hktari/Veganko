using Autofac;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Veganko.Common.Models.Products;
using Veganko.Common.Models.Users;
using Veganko.Extensions;
using Veganko.Models;
using Veganko.Models.Products;
using Veganko.Models.User;
using Veganko.Other;
using Veganko.Services;
using Veganko.Services.DB;
using Veganko.Services.Logging;
using Veganko.ViewModels.Products.Partial;
using Veganko.Views;
using Xamarin.Forms;

namespace Veganko.ViewModels.Products
{
    public class ProductListViewModel : BaseViewModel
    {
        public Command LoadItemsCommand { get; }
        public Command SearchClickedCommand => new Command(OnSearchClicked);
        public Command SearchBarcodeCommand => new Command(OnBarcodeSearch);

        public Command ProductSelectedCommand { get; set; }

        private UserRole userRole;
        public UserRole UserRole
        {
            get => userRole;
            set => SetProperty(ref userRole, value);
        }

        private string searchText = "";
        public string SearchText
        {
            get => searchText;
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

        private ObservableCollection<ProductViewModel> searchResult;
        public ObservableCollection<ProductViewModel> SearchResult
        {
            get => searchResult;
            set => SetProperty(ref searchResult, value);
        }

        private ObservableCollection<ProductClassifier> selectedProductClassifiers;
        public ObservableCollection<ProductClassifier> SelectedProductClassifiers
        {
            get => selectedProductClassifiers;
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
            get => selectedProductType;
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

        // The descriptions of the all the ProductType values
        public List<string> ProductTypePickerItems { get; } = EnumExtensionMethods.GetDescriptions(ProductType.NOT_SET);

        public List<ProductViewModel> Products { get; protected set; }

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

        private List<ProductViewModel> matchesByText;
        private bool ShouldNotifyUIOnly { get; set; }
        protected readonly IProductService productService;
        protected readonly IAccountService accountService;

        public ProductListViewModel()
        {
            productService = App.IoC.Resolve<IProductService>();
            accountService = App.IoC.Resolve<IAccountService>();
            
            MessagingCenter.Subscribe<NewProductViewModel, ProductViewModel>(this, NewProductViewModel.ProductAddedMsg, OnNewProductAdded);
            MessagingCenter.Subscribe<EditProductViewModel, ProductViewModel>(this, EditProductViewModel.ProductUpdatedMsg, OnProductUpdated);

            Title = "Iskanje";
            Products = new List<ProductViewModel>();
            SearchResult = new ObservableCollection<ProductViewModel>();
            SelectedProductClassifiers = new ObservableCollection<ProductClassifier>();
            SelectedProductType = ProductType.NOT_SET;
            UserRole = App.IoC.Resolve<IUserService>().CurrentUser.Role;

            LoadItemsCommand = new Command(async () =>
            {
                if (IsBusy)
                {
                    return;
                }

                IsBusy = true;

                try
                {
                    SearchText = string.Empty;
                    UnapplyFilters();
                    // TODO: handle pages
                    Products = await GetProducts();
                    SetSearchResults(Products);
                }
                catch (Exception ex)
                {
                    await App.CurrentPage.Err("Napaka pri nalaganju.");
                    Debug.WriteLine(ex);
                }
                finally
                {
                    IsBusy = false;
                }
            });

            ProductSelectedCommand = new Command(async (param) => await OnProductSelected((ProductViewModel)param));
        }

        private void OnProductUpdated(EditProductViewModel sender, ProductViewModel updatedProduct)
        {
            // Handles the case when the edited product is in my list but is a different instance (edit product via conflict)
            ProductViewModel prod = Products.FirstOrDefault(p => p.Id == updatedProduct.Id);
            if (prod != null && prod != updatedProduct)
            {
                prod.Update(updatedProduct);
            }
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

        public async Task DeleteProduct(ProductViewModel product)
        {
            Product productModel = new Product();
            product.MapToModel(productModel);
            await productService.DeleteAsync(productModel);
            Debug.Assert(Products != null);
            Products.Remove(product);
            SearchResult.Remove(product);
        }

        protected virtual async Task<List<ProductViewModel>> GetProducts()
        {
            PagedList<Product> products = await productService.AllAsync(pageSize: 1000, forceRefresh: true)
                                                              .ConfigureAwait(false);
            List<ProductViewModel> prodVMs = products.Items
                .OrderByDescending(p => p.AddedTimestamp)
                .Select(m => new ProductViewModel(m))
                .ToList();

            var productDBService = App.IoC.Resolve<IProductDBService>();
            IEnumerable<CachedProduct> seenProducts = await productDBService.GetAllSeenProducts()
                                                                            .ConfigureAwait(false);
            foreach (var seenProduct in seenProducts)
            {
                prodVMs
                    .FirstOrDefault(prod => prod.Id == seenProduct.ProductId)
                    ?.SetHasBeenSeen(true);
            }

            // Set products as seen in the background.
            Task.Run(() =>
            {
                try
                {
                    productDBService.SetProductsAsSeen(products.Items)
                                    .GetAwaiter()
                                    .GetResult();
                }
                catch (Exception ex)
                {
                    App.IoC.Resolve<ILogger>()
                            .LogException(new Exception("Failed to set products as seen.", ex));
                }
            }).FireAndForget();

            return prodVMs;
        }

        protected virtual Task OnProductSelected(ProductViewModel product)
        {
            return App.Navigation.PushAsync(new ProductDetailPage(new ProductDetailViewModel(product)));
        }

        protected void SetSearchResults(IEnumerable<ProductViewModel> items)
        {
            SearchResult = new ObservableCollection<ProductViewModel>(items);
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
                List<ProductViewModel> matches = null;
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    Debug.Assert(matchesByText != null);
                    matches = matchesByText.Where(
                        p =>
                        {
                            if (p.ProductClassifiers == null)
                            {
                                return false;
                            }
                            else if (p.Type != SelectedProductType)
                            {
                                return false;
                            }

                            if (SelectedProductClassifiers.Count > 0)
                            {
                                return p.ProductClassifiers.Intersect(SelectedProductClassifiers).Count() > 0;
                            }
                            else
                            {
                                return true;
                            }
                        }).ToList();
                }
                else
                {
                    matches = Products.Where(
                        p =>
                        {
                            if (p.ProductClassifiers == null)
                            {
                                return false;
                            }
                            else if (p.Type != SelectedProductType)
                            {
                                return false;
                            }

                            if (SelectedProductClassifiers.Count > 0)
                            {
                                return p.ProductClassifiers.Intersect(SelectedProductClassifiers).Count() > 0;
                            }
                            else
                            {
                                return true;
                            }
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

        private void OnNewProductAdded(NewProductViewModel sender, ProductViewModel product)
        {
            Products.Insert(0, product);
            SearchResult.Insert(0, product);
            UnapplyFilters(false);
        }
    }
}
