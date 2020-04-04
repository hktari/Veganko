using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.ViewModels.Users;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views.Profile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserProfileDetailPage : BaseContentPage
    {
        public UserProfileDetailPage(UserProfileDetailViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}