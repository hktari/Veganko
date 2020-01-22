using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.ViewModels.Stores;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views.Stores
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PickStorePage : BaseContentPage
    {
        public PickStorePage(PickStoreViewModel pickStoreVM)
        {
            InitializeComponent();
            BindingContext = pickStoreVM;
        }
    }
}