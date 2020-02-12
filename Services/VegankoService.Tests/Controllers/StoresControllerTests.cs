using Microsoft.VisualStudio.TestTools.UnitTesting;
using VegankoService.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using VegankoService.Data.Stores;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VegankoService.Models.Stores;
using VegankoService.Data;

namespace VegankoService.Controllers.UnitTests
{
    [TestClass()]
    public class StoresControllerTests
    {
        private MockStoresRepository repository;
        private StoresController controller;

        [TestInitialize]
        public void Init()
        {
            repository = new MockStoresRepository();
            controller = new StoresController(repository, new MockProductRepository());
        }

        [TestMethod()]
        public async Task GetStoreTest_NotFound()
        {
            Assert.IsInstanceOfType(
                await controller.GetStore("not-found"), typeof(NotFoundResult));
        }

        [TestMethod()]
        public async Task PutStoreTest_NoContent()
        {
            var store = new Store
            {
                Id = "store-id",
                ProductId = "product-id",
                Name = "store-name",
                Address = new Address { FormattedAddress = "store-address" },
                Price = 2.0
            };
            await repository.Create(store);

            store.Price = 4.50;
            store.Address = new Address { FormattedAddress = "new-store-address" };

            IActionResult result = await controller.PutStore(store.Id, store);

            var updatedStore = await repository.Get(store.Id);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            Assert.AreEqual(updatedStore.Name, store.Name);
            Assert.AreEqual(updatedStore.Price, store.Price);
            Assert.AreEqual(updatedStore.Address.FormattedAddress, store.Address.FormattedAddress);
        }

        [TestMethod()]
        public async Task PostStoreTest_CreatedAt()
        {
            IActionResult result = await controller.PostStore(
                new Store
                {
                    ProductId = "product-id",
                    Name = "store-name",
                    Address = new Address { FormattedAddress = "store-address" },
                    Price = 2.0
                });

            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
        }

        [TestMethod()]
        public async Task DeleteStoreTest_NotFound()
        {
            Assert.IsInstanceOfType(
                await controller.DeleteStore("not-found-id"), typeof(NotFoundResult));
        }
    }
}