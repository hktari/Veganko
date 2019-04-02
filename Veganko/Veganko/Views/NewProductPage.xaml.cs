using System;
using System.Collections.Generic;
using Veganko.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Product = Veganko.Models.Product;
using System.Linq;
using Veganko.Extensions;
using System.Collections.ObjectModel;
using Veganko.ViewModels;
using Plugin.Media;
using XamarinImageUploader;
using Veganko.Other;

namespace Veganko.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewProductPage : BaseContentPage
    {
        NewProductViewModel vm;
        
        public NewProductPage()
        {
            InitializeComponent();
            
            BindingContext = vm = new NewProductViewModel();
            TypePicker.SelectedIndexChanged += TypePickerSelectedIndexChanged;
            CameraButton.Clicked += async (sender, args) =>
            {
                var initialized = await CrossMedia.Current.Initialize();

                if (!initialized || !CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    DisplayAlert("No Camera", ":( No camera available.", "OK");
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "test.jpg"
                });

                if (file == null)
                    return;

                vm.Product.ImageName = await ImageManager.UploadImage(file.GetStream());
                await DisplayAlert("Alert", "Successfully uploaded image", "OK");
                ProductImage.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });
            };
        }

        private void TypePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            var type = (ProductType)Enum.Parse(typeof(ProductType), picker.SelectedItem as string, true);
            SelectableEnumImageView.Source = new ObservableCollection<ProductClassifier>(EnumConfiguration.ClassifierDictionary[type]) ?? new ObservableCollection<ProductClassifier>();
        }


        void Save_Clicked(object sender, EventArgs e)
        {
            var mainPage = App.Current.MainPage as TabbedPage;
            var productsNavPage = mainPage.Children[0];

            mainPage.CurrentPage = productsNavPage;

            var productsVM = (ProductViewModel)((ProductPage)((NavigationPage)productsNavPage).CurrentPage).BindingContext;
            productsVM.NewProductAddedCommand?.Execute(vm.Product);
        }

        async void Scan_Clicked(object sender, EventArgs e)
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();

            var result = await scanner.Scan();

            if (result != null)
            {
                vm.Product.Barcode = result.Text;
                await DisplayAlert("Obvestilo", "Skeniranje končano !", "OK");
            }
            else
            {
                await DisplayAlert("Obvestilo", "Napaka pri skeniranju !", "OK");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            vm.PageAppeared.Execute(null);
        }
    }
}