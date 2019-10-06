using Autofac;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Veganko.Models;
using Veganko.Models.User;
using Veganko.Other;
using Veganko.Services;
using Xamarin.Forms;

namespace Veganko.ViewModels
{
    public class NewProductViewModel : BaseViewModel
    {
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

        private ObservableCollection<ProductClassifier> productClassifiers;

        public ObservableCollection<ProductClassifier> ProductClassifiers
        {
            get => productClassifiers;
            set => SetProperty(ref productClassifiers, value);
        }

        public Command PageAppeared => new Command(OnPageAppeared);

        public Command TakeImageCommand => new Command(TakeImage);

        private async void TakeImage()
        {
            // TODO: test on other devices. If Ok remove unused lib from droid

            int maxPhotoWidthInPix = 1080;
            int maxPhotoHeightInDips = 300;
#if __ANDROID__
            byte[] data = await Droid.MainActivity.Context.DispatchTakePictureIntent(maxPhotoHeightInDips, maxPhotoWidthInPix);
            Product.ImageBase64Encoded = data;
            ProductImg = ImageSource.FromStream(() => new MemoryStream(data));
#else
        //    var initialized = await CrossMedia.Current.Initialize();

        //    if (!initialized || !CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
        //    {
        //        await App.Current.MainPage.DisplayAlert("No Camera", ":( No camera available.", "OK");
        //        return;
        //    }

        //    var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
        //    {
        //        Directory = "Sample",
        //        PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight,
        //        MaxWidthHeight = 720,
        //        CompressionQuality = 50,
        //        Name = Guid.NewGuid().ToString() + ".png"
        //    });

        //    if (file == null)
        //        return;

        //    //imageNameResult.Text = await ImageManager.UploadImage(file.GetStream());
        //    ProductImg = ImageSource.FromStream(() =>
        //    {
        //        var stream = file.GetStream();
        //        return stream;
        //    });

        //    using(Stream stream = file.GetStream())
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        stream.CopyTo(ms);
        //        Product.ImageBase64Encoded = ms.ToArray();
        //    }

        //    //await App.Current.MainPage.DisplayAlert("Alert", "Successfully uploaded image", "OK");
#endif
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
                Image = "raspeberry_meringue.jpg",
                //State = hasApprovalRights ? ProductState.Approved : ProductState.PendingApproval  // TODO: uncomment after testing
                ProductClassifiers = new ObservableCollection<ProductClassifier>(),
            };
            SelectedProductType = (ProductType)1;
            Barcode = null;
        }
    }
}
