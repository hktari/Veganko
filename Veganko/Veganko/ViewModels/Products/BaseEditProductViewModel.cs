using Autofac;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Veganko.Extensions;
using Veganko.Models;
using Veganko.Other;
using Veganko.Services;
using Veganko.Services.ImageManager;
using Veganko.ViewModels.Products.Partial;
using Xamarin.Forms;

namespace Veganko.ViewModels.Products
{
    public class BaseEditProductViewModel : BaseViewModel
    {
        public const int maxPhotoWidthInPix = 2160;
        public const int maxPhotoHeightInDips = 300;
        public const int thumbnailPhotoWidthInPix = 400;
        public const int thumbnailPhotoHeightInPix = 400;

        private readonly IImageProcessor imageProcessor;
        protected readonly IProductService productService;

        public BaseEditProductViewModel()
        {
            imageProcessor = App.IoC.Resolve<IImageProcessor>();
            productService = App.IoC.Resolve<IProductService>();
        }

        public BaseEditProductViewModel(ProductViewModel product)
        {
            imageProcessor = App.IoC.Resolve<IImageProcessor>();
            productService = App.IoC.Resolve<IProductService>();

            this.product = product;
            productImg = product.Image;
            SelectedProductType = product.Type;

            PhotoPicked = product.Image != null;
            BarcodePicked = product.Barcode != null;
        }

        private ProductViewModel product;
        public ProductViewModel Product
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

        protected ProductType selectedProductType;
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

                    if (product != null)
                    {
                        Product.Type = selectedProductType;
                    }
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

        public Command TakeImageCommand => new Command(HandleImageClicked);

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

        protected bool HasImageBeenChanged => ProductDetailImageData != null;

        protected byte[] ProductDetailImageData { get; private set; }
        
        protected byte[] ProductThumbnailImageData { get; private set; }

        protected void InitSelectedProductType(ProductType productType)
        {
            this.selectedProductType = productType;
        }

        protected async Task<Product> PostProductImages(Product product)
        {
            await GenerateThumbnail();
            return await productService.UpdateImagesAsync(product, ProductDetailImageData, ProductThumbnailImageData);
        }

        protected async Task GenerateThumbnail()
        {
            if (ProductDetailImageData == null)
            {
                throw new Exception("Can't generate thumbnail. Product image data is null.");
            }

            ProductThumbnailImageData = await imageProcessor.GenerateThumbnail(
                ProductDetailImageData,
                thumbnailPhotoHeightInPix,
                thumbnailPhotoWidthInPix);
        }

        private async void HandleImageClicked()
        {
            string result = await App.Current.MainPage.DisplayActionSheet("Fotografija", "Prekini", null, "Izberi iz galerije", "Slikaj");

            if (result == "Prekini")
            {
                return;
            }

            if (result == "Slikaj")
            {
                await TakeImage();
            }
            else
            {
                await PickImageFromGallery();
            }

            PhotoPicked = true;
        }

        private async Task PickImageFromGallery()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await App.CurrentPage.Err("Ni podpore za izbiranje fotografij");
                return;
            }

            MediaFile mediaFile = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
            {
                PhotoSize = PhotoSize.MaxWidthHeight,
                MaxWidthHeight = maxPhotoWidthInPix,
            });

            if (mediaFile == null)
            {
                await App.CurrentPage.Err("Nisem uspel naložiti fotografijo.");
                return;
            }

            LoadImage(mediaFile);
        }

        private async Task TakeImage()
        {
#if __ANDROID__
            byte[] data = await Droid.MainActivity.Context.DispatchTakePictureIntent(maxPhotoHeightInDips, maxPhotoWidthInPix);
            ProductDetailImageData = data;
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

            LoadImage(file);
#endif
        }

        private void LoadImage(MediaFile file)
        {
            ProductImg = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });

            using (Stream stream = file.GetStream())
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                ProductDetailImageData = ms.ToArray();
            }
        }
    }
}
