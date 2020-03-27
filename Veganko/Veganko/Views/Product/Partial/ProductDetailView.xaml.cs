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
	public partial class ProductDetailView : ContentView
	{
        public ProductDetailView ()
		{
			InitializeComponent ();
        }
    }
}