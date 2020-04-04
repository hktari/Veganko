using System.Linq;
using Veganko.ViewModels.Management;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views.Management
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManageUsersPage : BaseContentPage
    {
        private ManageUsersViewModel vm;

        public ManageUsersPage()
        {
            InitializeComponent();
            BindingContext = vm = new ManageUsersViewModel();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            if (string.IsNullOrEmpty(args.NewTextValue) && !string.IsNullOrEmpty(args.OldTextValue))
            {
                vm.RefreshUsersCommand?.Execute(null);
            }
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (args.SelectedItem == null)
            {
                return;
            }

            vm.UserSelectedCommand.Execute(args.SelectedItem);

            listView.SelectedItem = null;
        }
    }
}