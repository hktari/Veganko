using Autofac;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Veganko.Models.Stores;
using Veganko.Services;
using Veganko.Services.Logging;
using Veganko.Services.Products.Stores;
using Veganko.Views.Stores;
using Xamarin.Forms;

namespace Veganko.ViewModels.Stores
{
    public class StoreListViewModel : BaseViewModel
    {
        private readonly string productId;
        private readonly IStoresService storesService;
        private readonly ILogger logger;

        public StoreListViewModel(string productId)
        {
            this.productId = productId;
            storesService = App.IoC.Resolve<IStoresService>();
            logger = App.IoC.Resolve<ILogger>();
            HasEditingRights = App.IoC.Resolve<IUserService>().CurrentUser.IsManager();
            MessagingCenter.Subscribe<AddStoreViewModel, StoreViewModel>(this, AddStoreViewModel.StoreAddedMsg, OnStoreAdded);
            MessagingCenter.Subscribe<EditStoreViewModel, StoreViewModel>(this, EditStoreViewModel.StoreRemovedMsg, OnStoreRemoved);
        }

        private ObservableCollection<StoreViewModel> productStores;
        public ObservableCollection<StoreViewModel> ProductStores
        {
            get => productStores;
            set => SetProperty(ref productStores, value);
        }

        public Command AddStoreCommand => new Command(
            async () => await App.Navigation.PushModalAsync(
                new NavigationPage(
                    new AddStorePage(new AddStoreViewModel(productId)))));

        public Command StoreSelectedCommand => new Command(
            async () =>
            {
                if (SelectedStore == null)
                {
                    return;
                }

                await App.Navigation.PushModalAsync(
                    new NavigationPage(
                        new EditStorePage(new EditStoreViewModel(SelectedStore))));
                SelectedStore = null;
            });

        private StoreViewModel selectedStore;
        public StoreViewModel SelectedStore
        {
            get
            {
                return selectedStore;
            }
            set
            {
                SetProperty(ref selectedStore, value);
            }
        }

        private bool hasEditingRights;
        public bool HasEditingRights
        {
            get
            {
                return hasEditingRights;
            }
            set
            {
                SetProperty(ref hasEditingRights, value);
            }
        }

        public async Task LoadStores()
        {
            try
            {
                IsBusy = true;

                IEnumerable<Store> storeModels = await storesService.All(productId);
                ProductStores = new ObservableCollection<StoreViewModel>(
                    storeModels.Select(sm => new StoreViewModel(sm)));
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnStoreAdded(AddStoreViewModel sender, StoreViewModel newStore)
        {
            productStores?.Insert(0, newStore);
        }

        private void OnStoreRemoved(EditStoreViewModel sender, StoreViewModel removedStore)
        {
            ProductStores.Remove(removedStore);
        }
    }
}
