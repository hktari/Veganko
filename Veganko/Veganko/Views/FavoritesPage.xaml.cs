using Microsoft.WindowsAzure.MobileServices;
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
	public partial class FavoritesPage : BaseContentPage
	{
        FavoritesViewModel vm;

		public FavoritesPage ()
		{
			InitializeComponent ();

            BindingContext = this.vm = new FavoritesViewModel();
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            vm.Items = await App.MobileService.GetTable<TodoItem>().ToCollectionAsync();
        }
        private async void AddBtnClicked(object sender, EventArgs e)
        {
            TodoItem item = new TodoItem { Text = "Awesome item" };
            await App.MobileService.GetTable<TodoItem>().InsertAsync(item);
            vm.Items = await App.MobileService.GetTable<TodoItem>().ToCollectionAsync();
        }
    }
}