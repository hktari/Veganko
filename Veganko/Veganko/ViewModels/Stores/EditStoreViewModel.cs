using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Extensions;
using Veganko.Models.Stores;
using Veganko.Services.Http;
using Veganko.Services.Products.Stores;
using Veganko.ViewModels.Stores.Partial;
using Xamarin.Forms;

namespace Veganko.ViewModels.Stores
{
    public class EditStoreViewModel : BaseEditStoreViewModel
    {
        public const string StoreRemovedMsg = "StoreRemovedMsg";
        private IStoresService storeService;

        public EditStoreViewModel(StoreViewModel store)
            : base(store)
        {
            storeService = App.IoC.Resolve<IStoresService>();
        }

        public Command RemoveStoreCommand => new Command(
            async () =>
            {
                try
                {
                    IsBusy = true;

                    Store storeModel = new Store();
                    Store.MapToModel(storeModel);

                    await storeService.Remove(storeModel);
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

        public Command CancelCommand => new Command(() => App.Navigation.PopModalAsync());

        public Command SaveStoreCommand => new Command(
            async () =>
            {
                if (!Store.Validate())
                {
                    return;
                }

                try
                {
                    IsBusy = true;

                    Store storeModel = new Store();
                    Store.MapToModel(storeModel);

                    await storeService.Update(storeModel);
                    await App.Navigation.PopModalAsync();
                }
                catch (ServiceException se)
                {
                    await App.CurrentPage.Err("Spremembe niso bile shranjene.", se);
                }
                finally
                {
                    IsBusy = false;
                }
            });
    }
}
