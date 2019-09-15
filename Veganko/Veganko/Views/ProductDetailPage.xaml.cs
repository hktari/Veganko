using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Extensions;
using Veganko.Services.Http;
using Veganko.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProductDetailPage : BaseContentPage
	{
        ProductDetailViewModel vm;
		public ProductDetailPage (ProductDetailViewModel vm)
		{
			InitializeComponent ();
            BindingContext = this.vm = vm;
		}
        public ProductDetailPage() : this(null) { } // Satisfy the Xamarin.Forms previewer

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                vm.IsBusy = true;
                await vm.RefreshComments();
                //vm.RefreshIsFavorite(); // TODO: remove ?
            }
            catch (ServiceException ex)
            {
                await this.Err(ex.StatusDescription);
            }
            finally
            {
                vm.IsBusy = false;
            }
        }

        private async void OnDeleteCommentClicked(object sender, EventArgs args)
        {
            await this.Err("DEELTEING COMMENT!");
        }

        private async void OnSendCommentClicked(object sender, EventArgs args)
        {
            if (string.IsNullOrWhiteSpace(vm.NewComment.Text) || vm.NewComment.Rating == 0)
            {
                await this.Err("Prosim vnesi text in oceno.");
                return;
            }

            try
            {
                vm.IsBusy = true;
                await vm.SendComment();
            }
            catch (ServiceException ex)
            {
                await this.Err(ex.Response);
            }
            finally
            {
                vm.IsBusy = false;
            }
        }
    }
}