using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Veganko.ViewModels.Stores;

namespace UnitTests.Shared.ViewModels.Stores
{
    [TestClass]
    public class EditStoreViewModelTests
    {
        [TestMethod]
        public void StorePropertyChangedEvent_UpdatesIsDirty()
        {
            StoreViewModel store = new StoreViewModel("product-id");
            EditStoreViewModel editStoreVM = new EditStoreViewModel(store);

            editStoreVM.OnPageAppearing();
            store.Name.Value = "product-name";

            Assert.IsTrue(editStoreVM.IsDirty);
        }
    }
}
