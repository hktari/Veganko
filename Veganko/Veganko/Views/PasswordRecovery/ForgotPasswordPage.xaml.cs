using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Extensions;
using Veganko.Services.Http;
using Veganko.ViewModels.PasswordRecovery;
using Veganko.Views.PasswordRecovery;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views.PasswordRecovery
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForgotPasswordPage : ContentPage
    {
        private PasswordRecoveryViewModel vm;

        public ForgotPasswordPage()
        {
            InitializeComponent();
            BindingContext = this.vm = new PasswordRecoveryViewModel();
        }
    }
}