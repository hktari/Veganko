using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.ViewModels.Profile;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views.Profile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectAvatarPage : BaseContentPage
    {
        private SelectAvatarViewModel vm;

        public SelectAvatarPage(SelectAvatarViewModel vm)
        {
            BindingContext = this.vm = vm;
            InitializeComponent();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            await vm.Save();
            await Navigation.PopModalAsync();
        }
    }
}