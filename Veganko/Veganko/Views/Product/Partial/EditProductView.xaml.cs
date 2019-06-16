using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media;
using XamarinImageUploader;
using Veganko.Other;

namespace Veganko.Views.Product.Partial
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditProductView : ContentView
	{
		public EditProductView ()
		{
			InitializeComponent ();

            CameraButton.Clicked += async (sender, args) =>
            {
                var initialized = await CrossMedia.Current.Initialize();

                if (!initialized || !CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    App.Current.MainPage.DisplayAlert("No Camera", ":( No camera available.", "OK");
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "test.jpg"
                });

                if (file == null)
                    return;

                //imageNameResult.Text = await ImageManager.UploadImage(file.GetStream());
                imageNameResult.Text = "unknown";
                CameraButton.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });

                await App.Current.MainPage.DisplayAlert("Alert", "Successfully uploaded image", "OK");
            };
        }

        async void Scan_Clicked(object sender, EventArgs e)
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();

            var result = await scanner.Scan();

            if (result != null)
            {
                barcodeScanResult.Text = result.Text;
                await App.Current.MainPage.DisplayAlert("Obvestilo", "Skeniranje končano !", "OK");
            }
            else
            {
                barcodeScanResult.Text = null;
                await App.Current.MainPage.DisplayAlert("Obvestilo", "Napaka pri skeniranju !", "OK");
            }
        }
    }
}