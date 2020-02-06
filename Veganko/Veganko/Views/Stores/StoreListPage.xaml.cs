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
    public partial class StoreListPage : BaseContentPage
    {
        private readonly StoreListViewModel vm;

        public StoreListPage(StoreListViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            this.vm = vm;
        }

        protected async override void CustomOnAppearing()
        {
            if (vm.ProductStores == null)
            {
                await vm.LoadStores().ConfigureAwait(false);
            }
        }
    }
}