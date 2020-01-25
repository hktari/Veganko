using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Views.Stores;
using Xamarin.Forms;

namespace Veganko.ViewModels.Stores.Partial
{
    public class BaseEditStoreViewModel : BaseViewModel
    {
        private PickStoreViewModel pickStoreVM;

        public BaseEditStoreViewModel(StoreViewModel store)
        {
            Store = store;
            pickStoreVM = new PickStoreViewModel();
            MessagingCenter.Subscribe<PickStoreViewModel, PickStoreViewModel.PickStoreResult>(
                this,
                PickStoreViewModel.StorePickedMsg,
                OnStorePicked,
                pickStoreVM);
        }

        public StoreViewModel Store { get; }

        private void OnStorePicked(PickStoreViewModel sender, PickStoreViewModel.PickStoreResult storeData)
        {
            Store.Update(storeData);
            Store.Validate();
        }

        public Command OpenStorePickerCommand => new Command(
            async () => await App.Navigation.PushModalAsync(new PickStorePage(pickStoreVM)));
    }
}
