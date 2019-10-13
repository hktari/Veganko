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
        public static readonly BindableProperty TakeImageCommandProperty
            = BindableProperty.Create(nameof(TakeImageCommand), typeof(Command), typeof(EditProductView));
        
        public Command TakeImageCommand
        {
            get => (Command)GetValue(TakeImageCommandProperty);
            set => SetValue(TakeImageCommandProperty, value);
        }

        public EditProductView ()
		{
			InitializeComponent ();
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

        private void OnCameraBtnClicked(object sender, EventArgs e)
        {
            TakeImageCommand?.Execute(null);
        }
    }
}