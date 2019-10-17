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

        private void OnCameraBtnClicked(object sender, EventArgs e)
        {
            TakeImageCommand?.Execute(null);
        }
    }
}