using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Loginpage : ContentPage
	{
        LoginViewModel vm;
		public Loginpage ()
		{
			InitializeComponent ();
            BindingContext = this.vm = new LoginViewModel();
		}
	}
}