using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Veganko;
using Autofac;
using Veganko.Services;
using Veganko.Models;
using Veganko.ViewModels.Products;
using Veganko.Services.DB;
using Veganko.Models.Products;

namespace UnitTests.Shared.ViewModels.Products
{
    [TestClass]
    public class ProductListViewModelTests
    {
        private ProductListViewModel vm;

        [TestInitialize]
        public void Init()
        {
            vm = new ProductListViewModel();
            var productService = (MockProductDataStore)App.IoC.Resolve<IProductService>();
            productService.Items = new List<Product>
            {
                new Product { Id = "0", AddedTimestamp = DateTime.Now - TimeSpan.FromHours(2) },
                new Product{ Id = "2", AddedTimestamp = DateTime.Now - TimeSpan.FromDays(2.5) },
                new Product{ Id = "1", AddedTimestamp = DateTime.Now - TimeSpan.FromDays(1.1) }
            };
        }

        [TestMethod]
        public void TestProducts_AreOrderedNewestFirst()
        {
            vm.LoadItemsCommand.Execute(null);

            Assert.AreEqual("0", vm.Products[0].Id);
            Assert.AreEqual("1", vm.Products[1].Id);
            Assert.AreEqual("2", vm.Products[2].Id);
        }

        [TestMethod]
        public void TestProducts_IsNew()
        {
            vm.LoadItemsCommand.Execute(null);
            Assert.IsTrue(vm.Products[0].IsNew);
            Assert.IsTrue(vm.Products[1].IsNew);
            Assert.IsFalse(vm.Products[2].IsNew);
        }

        [TestMethod]
        public void TestProducts_IsNewWHasBeenSeen()
        {
            var productDBService = (MockProductDBService)App.IoC.Resolve<IProductDBService>();

            productDBService.Products = new List<CachedProduct>
            {
                new CachedProduct { ProductId = "1" }
            };

            vm.LoadItemsCommand.Execute(null);

            Assert.IsTrue(vm.Products[0].IsNew);
            Assert.IsFalse(vm.Products[1].IsNew);
            Assert.IsFalse(vm.Products[2].IsNew);
        }
    }
}
