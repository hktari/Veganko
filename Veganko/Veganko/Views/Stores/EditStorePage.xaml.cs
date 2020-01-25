using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views.Stores
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditStorePage : BaseContentPage
    {
        public EditStorePage(ViewModels.Stores.EditStoreViewModel editStoreViewModel)
            : base(true)
        {
            InitializeComponent();
            BindingContext = editStoreViewModel;
        }
    }
}