using Autofac;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Veganko.Extensions;
using Veganko.Models;
using Veganko.Models.User;
using Veganko.Other;
using Veganko.Services;
using Veganko.Services.Http;
using Veganko.Views;
using Xamarin.Forms;

namespace Veganko.ViewModels
{
    public class NewProductViewModel : BaseViewModel
    {
        public const string ProductAddedMsg = "ProductAdded";

        private Product product;
        public Product Product
        {
            get
            {
                return product;
            }
            set
            {
                SetProperty(ref product, value);
            }
        }

        private ImageSource productImg;
        public ImageSource ProductImg
        {
            get
            {
                return productImg;
            }
            set
            {
                SetProperty(ref productImg, value);
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
                    ProductClassifiers?.Clear();
                    ProductClassifiers = new ObservableCollection<ProductClassifier>(EnumConfiguration.ClassifierDictionary[value]);
                    Product.Type = selectedProductType;
                }
            }
        }

        private bool photoPicked;
        public bool PhotoPicked
        {
            set => SetProperty(ref photoPicked, value);
            get => photoPicked;
        }

        private bool barcodePicked;
        public bool BarcodePicked
        {
            get => barcodePicked;
            set => SetProperty(ref barcodePicked, value);
        }

        private ObservableCollection<ProductClassifier> productClassifiers;

        public ObservableCollection<ProductClassifier> ProductClassifiers
        {
            get => productClassifiers;
            set => SetProperty(ref productClassifiers, value);
        }

        public Command PageAppeared => new Command(OnPageAppeared);

        public Command TakeImageCommand => new Command(TakeImage);

        public Command TakeBarcodeCommand => new Command(
            async () => 
            {
                var scanner = new ZXing.Mobile.MobileBarcodeScanner();

                var result = await scanner.Scan();

                if (result != null)
                {
                    Barcode = result.Text;
                    BarcodePicked = true;
                    await App.Current.MainPage.DisplayAlert("Obvestilo", "Skeniranje končano !", "OK");
                }
                else
                {
                    Barcode = null;
                    BarcodePicked = false;
                    await App.Current.MainPage.DisplayAlert("Obvestilo", "Napaka pri skeniranju !", "OK");
                }
            });

        public Command SaveCommand => new Command(
            async () => 
            {
                try
                {
                    product.Type = selectedProductType;
                    product = await productService.AddAsync(product);
                    ((MainPage)App.Current.MainPage).SetCurrentTab(0);
                    MessagingCenter.Send(this, ProductAddedMsg, product);
                }
                catch (ServiceException ex)
                {
                    await App.CurrentPage.Err("Napak pri dodajanju: " + ex.Response);
                }
            });

        private IProductService productService;

        public NewProductViewModel()
        {
            productService = App.IoC.Resolve<IProductService>();
        }

        private async void TakeImage()
        {
            int maxPhotoWidthInPix = 1080;
            int maxPhotoHeightInDips = 300;

#if __ANDROID__
            byte[] data = await Droid.MainActivity.Context.DispatchTakePictureIntent(maxPhotoHeightInDips, maxPhotoWidthInPix);
            Product.ImageBase64Encoded = data;
            ProductImg = ImageSource.FromStream(() => new MemoryStream(data));
#else
            var initialized = await CrossMedia.Current.Initialize();

            if (!initialized || !CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await App.Current.MainPage.DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Pictures",
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight,
                MaxWidthHeight = maxPhotoHeightInDips,
                CompressionQuality = 100,
                Name = Guid.NewGuid().ToString() + ".png"
            });

            if (file == null)
                return;

            ProductImg = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });

            using(Stream stream = file.GetStream())
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                Product.ImageBase64Encoded = ms.ToArray();
            }
#endif

            PhotoPicked = true;
        }

        public string Barcode
        {
            get
            {
                return Product?.Barcode;
            }
            set
            {
                if (Product?.Barcode == value)
                    return;
                Product.Barcode = value;
                OnPropertyChanged(nameof(Barcode));
            }
        }

        private void OnPageAppeared(object parameter)
        {
            InitProduct();
        }

        private void InitProduct()
        {
            var user = App.IoC.Resolve<IUserService>().CurrentUser;
            var mask = UserAccessRights.ProductsDelete;

            Debug.Assert(user != null);
            // TODO:
            var hasApprovalRights = (user.Role.ToUAC() & mask) == mask;

            Product = new Product
            {
                //State = hasApprovalRights ? ProductState.Approved : ProductState.PendingApproval  // TODO: uncomment after testing
                ProductClassifiers = new ObservableCollection<ProductClassifier>(),
            };
            SelectedProductType = (ProductType)1;
            Barcode = null;
            PhotoPicked = false;
            BarcodePicked = false;
        }
    }
}
