using Autofac;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Veganko.Models.Stores;
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
            MessagingCenter.Subscribe<AddStoreViewModel, Store>(this, AddStoreViewModel.StoreAddedMsg, OnStoreAdded);
        }

        private ObservableCollection<Store> productStores;
        public ObservableCollection<Store> ProductStores
        {
            get => productStores;
            set => SetProperty(ref productStores, value);
        }

        public Command AddStoreCommand => new Command(
            async () => await App.Navigation.PushModalAsync(
                new NavigationPage(
                    new AddStorePage(new AddStoreViewModel(productId)))));

        public async Task LoadStores()
        {
            try
            {
                IsBusy = true;
                ProductStores = new ObservableCollection<Store>(
                    await storesService.All(productId));
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

        private void OnStoreAdded(AddStoreViewModel sender, Store newStore)
        {
            productStores?.Insert(0, newStore);
        }
    }
}
