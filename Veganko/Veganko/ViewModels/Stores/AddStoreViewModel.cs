using Veganko.Validations;
using Veganko.Views.Stores;
using Xamarin.Forms;
using Autofac;
using Veganko.Services.Products.Stores;
using Veganko.Extensions;
using Veganko.Services.Http;
using Veganko.Services.Logging;
using Veganko.Models.Stores;
using Veganko.ViewModels.Stores.Partial;

namespace Veganko.ViewModels.Stores
{
    public class AddStoreViewModel : BaseEditStoreViewModel
    {
        public const string StoreAddedMsg = "StoreAddedMsg";
        private readonly IStoresService storesService;
        private readonly ILogger logger;

        public AddStoreViewModel(string productId)
            : base(new StoreViewModel(productId))
        {
            storesService = App.IoC.Resolve<IStoresService>();
            logger = App.IoC.Resolve<ILogger>();
        }

        // TODO: when store picked to upper the name
        // TODO: allow editing of the name if string !IsNullOrWhiteSpace
        // TODO: after submited, save to cache as "recently added"
// TODO: disable naslov field

        // TODO: compare name with list of verified (common) store names. If NO exact match and suggestions not empty SHOW
        //          when picked replace name with picked item

        private Command submitCommand;
        public Command SubmitCommand => submitCommand ?? (submitCommand = new Command(
            async () =>
            {
                if (!Store.Validate())
                {
                    return;
                }

                try
                {
                    IsBusy = true;

                    var storeModel = new Store();
                    Store.MapToModel(storeModel);

                    storeModel = await storesService.Add(storeModel);
                    Store.Update(storeModel);

                    await App.Navigation.PopModalAsync();

                    MessagingCenter.Send(this, StoreAddedMsg, Store);
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
    }
}
