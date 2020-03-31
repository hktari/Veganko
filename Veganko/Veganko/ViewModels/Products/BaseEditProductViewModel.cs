using Autofac;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Veganko.Common.Models.Products;
using Veganko.Extensions;
using Veganko.Models;
using Veganko.Other;
using Veganko.Services;
using Veganko.Services.Http.Errors;
using Veganko.Services.ImageManager;
using Veganko.Services.Logging;
using Veganko.Services.Products.ProductModRequests;
using Veganko.Validations;
using Veganko.ViewModels.Products.Partial;
using Veganko.Views;
using Xamarin.Forms;
using ProductModel = Veganko.Common.Models.Products.Product;

namespace Veganko.ViewModels.Products
{
    public class BaseEditProductViewModel : BaseViewModel
    {
        /// <summary>
        /// Identifier for the message, that a product has been added or edited by a user whose role is member.
        /// </summary>
        public const string ProductModReqAddedMsg = "ProductModReqAdded";

        public const int maxPhotoWidthHeightInPix = 2160;
        //public const int maxPhotoHeightInDips = 300;
        public const int thumbnailPhotoWidthInPix = 400;
        public const int thumbnailPhotoHeightInPix = 400;

        private readonly IImageProcessor imageProcessor;
        protected readonly IProductService productService;
        protected readonly IProductModRequestService productModReqService;
        protected readonly IUserService userService;

        public BaseEditProductViewModel()
        {
            imageProcessor = App.IoC.Resolve<IImageProcessor>();
            productService = App.IoC.Resolve<IProductService>();
            productModReqService = App.IoC.Resolve<IProductModRequestService>();
            userService = App.IoC.Resolve<IUserService>();

            SelectedProductType = ProductType.Ostalo;
            SetupValidations();
        }

        public BaseEditProductViewModel(ProductViewModel product)
        {
            imageProcessor = App.IoC.Resolve<IImageProcessor>();
            productService = App.IoC.Resolve<IProductService>();
            productModReqService = App.IoC.Resolve<IProductModRequestService>();
            userService = App.IoC.Resolve<IUserService>();

            this.product = product;

            SetupValidations();

            SelectedProductType = product.Type;
            PhotoPicked = product.Image != null;
            BarcodePicked = product.Barcode != null;
            Name.Value = product.Name;
            ProductImg.Value = product.Image;
        }

        private ProductViewModel product;
        public ProductViewModel Product
        {
            get => product;
            set => SetProperty(ref product, value);
        }

        private ValidatableObject<string> name;
        public ValidatableObject<string> Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private ValidatableObject<ImageSource> productImg;
        public ValidatableObject<ImageSource> ProductImg
        {
            get => productImg;
            set => SetProperty(ref productImg, value);
        }

        // The descriptions of the all the ProductType values excluding NOT_SET
        public List<string> ProductTypePickerItems { get; } = EnumExtensionMethods.GetDescriptions(ProductType.NOT_SET).Skip(1).ToList();

        protected ProductType selectedProductType;
        public ProductType SelectedProductType
        {
            get => selectedProductType;
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
            get => Product?.Barcode;
            set
            {
                if (Product?.Barcode == value)
                {
                    return;
                }

                Product.Barcode = value;
                OnPropertyChanged(nameof(Barcode));
            }
        }

        protected bool HasImageBeenChanged => ProductDetailImageData != null;

        protected byte[] ProductDetailImageData { get; private set; }

        protected byte[] ProductThumbnailImageData { get; private set; }

        protected ILogger Logger => App.IoC.Resolve<ILogger>();

        protected async Task<Product> PostProductImages(Product product)
        {
            await GenerateThumbnail();
            return await productService.UpdateImagesAsync(product, ProductDetailImageData, ProductThumbnailImageData);
        }

        protected async Task<ProductModRequestDTO> PostProductImages(ProductModRequestDTO product)
        {
            await GenerateThumbnail();
            return await productModReqService.UpdateImagesAsync(product, ProductDetailImageData, ProductThumbnailImageData);
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

        protected async Task HandleDuplicateError(ServiceConflictException<Product> sce)
        {
            await App.CurrentPage.Err("Produkt s to črtno kodo že obstaja");

            App.SetCurrentTab(0);
            await App.Navigation.PopToRootAsync();

            await App.Navigation.PushAsync(
                new ProductDetailPage(
                    new ProductDetailViewModel(sce.RequestConflict.ConflictingItem)));
        }

        protected bool ValidateFields()
        {
            bool isValid = true;
            //isValid &= SelectedProductType > ProductType.NOT_SET;
            isValid &= Name.Validate();
            isValid &= ProductImg.Validate();

            return isValid;
        }

        protected Product CreateModel()
        {
            Product updatedProduct = new Product();
            Product.MapToModel(updatedProduct);
            updatedProduct.Name = Name.Value;
            return updatedProduct;
        }

        protected List<string> GetChangedFields(ProductModel originalProduct)
        {
            ProductModel updatedProduct = CreateModel();

            List<string> changedFields = new List<string>();
            if (originalProduct.Name != updatedProduct.Name)
            {
                changedFields.Add(nameof(ProductModel.Name));
            }

            if (originalProduct.Brand != updatedProduct.Brand)
            {
                changedFields.Add(nameof(ProductModel.Brand));
            }

            if (originalProduct.Barcode != updatedProduct.Barcode)
            {
                changedFields.Add(nameof(ProductModel.Barcode));
            }

            if (HasImageBeenChanged)
            {
                changedFields.Add(nameof(ProductModel.ImageName));
            }

            if (originalProduct.Description != updatedProduct.Description)
            {
                changedFields.Add(nameof(ProductModel.Description));
            }

            if (originalProduct.ProductClassifiers != updatedProduct.ProductClassifiers)
            {
                changedFields.Add(nameof(ProductModel.ProductClassifiers));
            }

            if (originalProduct.Type != updatedProduct.Type)
            {
                changedFields.Add(nameof(ProductModel.Type));
            }

            return changedFields;
        }

        private void SetupValidations()
        {
            Name = new ValidatableObject<string>();
            Name.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = "Ime je obvezno." });
            ProductImg = new ValidatableObject<ImageSource>();
            ProductImg.Validations.Add(new IsImageNotEmpty { ValidationMessage = "Slika je obvezna." });
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
                MaxWidthHeight = maxPhotoWidthHeightInPix,
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
            // My low end phone automatically terminates process when cross media library is used.
            if (Droid.MainActivity.OSVersion <= 21)
            {
                byte[] data = await Droid.MainActivity.Context.DispatchTakePictureIntent(maxPhotoWidthHeightInPix);
                ProductDetailImageData = data;
                ProductImg.Value = ImageSource.FromStream(() => new MemoryStream(data));
            }
            else
            {
                await TakeImageWithCrossMedia();
            }
#else
            await TakeImageWithCrossMedia();
#endif
        }

        private async Task TakeImageWithCrossMedia()
        {
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
                MaxWidthHeight = maxPhotoWidthHeightInPix,
                CompressionQuality = 85,
                Name = Guid.NewGuid().ToString() + ".jpg"
            });

            if (file == null)
            {
                return;
            }

            LoadImage(file);
        }

        private void LoadImage(MediaFile file)
        {
            ProductImg.Value = ImageSource.FromStream(() =>
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
