using Veganko.Validations;
using Veganko.Views.Stores;
using Xamarin.Forms;
using Autofac;
using Veganko.Services.Products.Stores;
using Veganko.Extensions;
using Veganko.Services.Http;
using Veganko.Services.Logging;
using Veganko.Models.Stores;

namespace Veganko.ViewModels.Stores
{
    public class AddStoreViewModel : BaseViewModel
    {
        public const string StoreAddedMsg = "StoreAddedMsg";
        private readonly string productId;
        private readonly IStoresService storesService;
        private readonly ILogger logger;
        private PickStoreViewModel pickStoreVM;
        private Store store;

        public AddStoreViewModel(string productId)
        {
            this.productId = productId;
            storesService = App.IoC.Resolve<IStoresService>();
            logger = App.IoC.Resolve<ILogger>();
            StoreName = new ValidatableObject<string>();
            StoreAddress = new ValidatableObject<string>();
            ProductPrice = new ValidatableObject<double>();

            StoreName.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = "Polje je obvezno" });
            StoreAddress.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = "Polje je obvezno" });
            ProductPrice.Validations.Add(new IsDoubleInRange(0.01) { ValidationMessage = "Dodaj še ceno" });

            pickStoreVM = new PickStoreViewModel();
            MessagingCenter.Subscribe<PickStoreViewModel, Store>(
                this,
                PickStoreViewModel.StorePickedMsg,
                OnStorePicked,
                pickStoreVM);
        }

        private void OnStorePicked(PickStoreViewModel sender, Store storeData)
        {
            store = storeData;
            StoreName.Value = storeData.Name;
            StoreAddress.Value = storeData.Address.FormattedAddress;
            CoordinatesFound = storeData.Coordinates != null;
        }

        public Command OpenStorePickerCommand => new Command(
            async () => await App.Navigation.PushModalAsync(new PickStorePage(pickStoreVM)));

        private Command submitCommand;
        public Command SubmitCommand => submitCommand ?? (submitCommand = new Command(
            async () =>
            {
                bool isValid = StoreName.Validate() && StoreAddress.Validate() && ProductPrice.Validate();
                if (!isValid)
                {
                    return;
                }

                try
                {
                    IsBusy = true;

                    if (store == null)
                    {
                        store = new Store();
                        store.Address = new Address();
                    }

                    store.Address.FormattedAddress = StoreAddress.Value;
                    store.Name = StoreName.Value;
                    store.Price = ProductPrice.Value;

                    store = await storesService.Add(store);

                    await App.Navigation.PopModalAsync();

                    MessagingCenter.Send(this, StoreAddedMsg, store);
                }
                catch (ServiceException se)
                {
                    await App.CurrentPage.Err("Napaka pri dodajanju.", se);
                    logger.LogException(se);
                }
                finally
                {
                    IsBusy = false;
                }
            }));

        private Command cancelCommand;
        public Command CancelCommand => cancelCommand ?? (cancelCommand = new Command(
            () => App.Navigation.PopModalAsync())); 

        public ValidatableObject<string> StoreName { get; }

        public ValidatableObject<string> StoreAddress { get; }

        public ValidatableObject<double> ProductPrice { get; }

        private bool coordinatesFound;

        public bool CoordinatesFound
        {
            get
            {
                return coordinatesFound;
            }
            set
            {
                SetProperty(ref coordinatesFound, value);
            }
        }
        
        public Coordinates StoreCoordinates { get; private set; }
    }
}
