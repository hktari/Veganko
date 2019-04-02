using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Other;
using Veganko.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinImageUploader;
using Plugin.Media;

namespace Veganko.Views.Product
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ApproveProductPage : BaseContentPage
	{
        private ApproveProductViewModel vm;

        public ApproveProductPage (ApproveProductViewModel vm)
		{
			InitializeComponent ();
            BindingContext = this.vm = vm;
            TypePicker.SelectedIndexChanged += TypePickerSelectedIndexChanged;
            CameraButton.Clicked += async (sender, args) =>
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
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

        private async void OnApproveProductClicked(object sender, EventArgs arg)
        {
            var result = await DisplayActionSheet("Are you sure you wish to approve this product ?", "Cancel", "Yes");

            if (result == "Yes")
            {
                vm.ApproveProductCommand?.Execute(null);

                await Navigation.PopAsync();
            }
        }

        private async void OnDeleteProductClicked(object sender, EventArgs arg)
        {
            var result = await DisplayActionSheet("Are you sure you wish to delete this product ?", "Cancel", "Yes");

            if (result == "Yes")
            {
                vm.DeleteProductCommand?.Execute(null);

                await Navigation.PopAsync();
            }
        }
    }
}