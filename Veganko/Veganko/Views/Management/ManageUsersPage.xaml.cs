using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Extensions;
using Veganko.Services.Http;
using Veganko.ViewModels;
using Veganko.ViewModels.Management;
using Veganko.ViewModels.Products;
using Veganko.ViewModels.Products.Partial;
using Veganko.Views.Product;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views.Management
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManageUsersPage : BaseContentPage
    {
        public ManageUsersPage()
        {
            InitializeComponent();
            BindingContext = new ManageUsersViewModel();
        }
    }
}