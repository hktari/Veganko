using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Extensions;
using Veganko.Models.Stores;
using Veganko.Services.Http;
using Veganko.Services.Products.Stores;
using Xamarin.Forms;

namespace Veganko.ViewModels.Stores
{
    public class EditStoreViewModel : BaseViewModel
    {
        public const string StoreRemovedMsg = "StoreRemovedMsg";
        private IStoresService storeService;

        public EditStoreViewModel(Store store)
        {
            Store = store;
            storeService = App.IoC.Resolve<IStoresService>();
        }

        public Store Store { get; }

        public Command RemoveStoreCommand => new Command(
            async () =>
            {
                try
                {
                    IsBusy = true;
                    await storeService.Remove(Store);
                    MessagingCenter.Send(this, StoreRemovedMsg, Store);
                    await App.Navigation.PopModalAsync();
                }
                catch (ServiceException se)
                {
                    await App.CurrentPage.Err("Brisanje ni uspelo.", se).ConfigureAwait(false);
                }
                finally
                {
                    IsBusy = false;
                }
            });
    }
}
