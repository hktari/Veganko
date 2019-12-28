using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.ViewModels.Account;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views.Account
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FinishRegistrationInstructPage : ContentPage
    {
        public FinishRegistrationInstructPage()
        {
            InitializeComponent();
            BindingContext = new FinishRegistrationInstructViewModel();
        }
    }
}