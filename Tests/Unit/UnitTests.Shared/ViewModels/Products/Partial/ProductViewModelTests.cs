using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Veganko.Models;
using Veganko.ViewModels.Products.Partial;

namespace UnitTests.Shared.ViewModels.Products.Partial
{
    [TestClass]
    public class ProductViewModelTests
    {
        [TestMethod]
        public void TestUpdateArgsProductViewModel()
        {
            ProductViewModel productVM = new ProductViewModel();
            ProductViewModel updatedProdVM = CreateDefaultVM();

            productVM.Update(updatedProdVM);
            
            AssertProductVMEquality(productVM, updatedProdVM);
        }

        [TestMethod]
        public void TestProductViewModelArgsProductViewModel()
        {
            ProductViewModel updatedProdVM = CreateDefaultVM();
            ProductViewModel prodVM = new ProductViewModel(updatedProdVM);

            AssertProductVMEquality(prodVM, updatedProdVM);
        }

        [TestMethod]
        public void TestUpdateArgsProduct()
        {
            Product product = CreateDefaultProduct();
            ProductViewModel prodVM = new ProductViewModel();

            prodVM.Update(product);

            AssertProductEquality(product, prodVM);
        }

        [TestMethod]
        public void TestMapToModelArgsProduct()
        {
            Product product = new Product();
            ProductViewModel prodVM = CreateDefaultVM();

            prodVM.MapToModel(product);

            AssertProductEquality(product, prodVM);
        }

        private static void AssertProductVMEquality(ProductViewModel productVM, ProductViewModel updatedProdVM)
        {
            Assert.AreEqual(updatedProdVM.Barcode, productVM.Barcode);
            Assert.AreEqual(updatedProdVM.Brand, productVM.Brand);
            Assert.AreEqual(updatedProdVM.Description, productVM.Description);
            Assert.AreEqual(updatedProdVM.Id, productVM.Id);
            Assert.AreEqual(updatedProdVM.Image, productVM.Image);
            Assert.AreEqual(updatedProdVM.Name, productVM.Name);
            Assert.AreEqual(updatedProdVM.ThumbnailImage, productVM.ThumbnailImage);
            Assert.AreEqual(updatedProdVM.Type, productVM.Type);

            Assert.AreNotEqual(updatedProdVM.ProductClassifiers, productVM.ProductClassifiers);
            Assert.IsTrue(updatedProdVM.ProductClassifiers
                .Zip(productVM.ProductClassifiers, (first, second) => first == second)
                .All(@bool => @bool));
        }

        private static void AssertProductEquality(Product product, ProductViewModel updatedProdVM)
        {
            Assert.AreEqual(updatedProdVM.Barcode, product.Barcode);
            Assert.AreEqual(updatedProdVM.Brand, product.Brand);
            Assert.AreEqual(updatedProdVM.Description, product.Description);
            Assert.AreEqual(updatedProdVM.Id, product.Id);
            Assert.AreEqual(updatedProdVM.Name, product.Name);
            Assert.AreEqual(updatedProdVM.Type, product.Type);

            Assert.AreNotEqual(updatedProdVM.ProductClassifiers, product.ProductClassifiers);
            Assert.IsTrue(updatedProdVM.ProductClassifiers
                .Zip(product.ProductClassifiers, (first, second) => first == second)
                .All(@bool => @bool));
        }

        private static ProductViewModel CreateDefaultVM()
        {
            return new ProductViewModel
            {
                Barcode = "barcode",
                Description = "desc",
                Brand = "brand",
                Id = "some-id",
                Name = "prod-name",
                Type = ProductType.Hrana,
                ProductClassifiers = new ObservableCollection<ProductClassifier> { ProductClassifier.Bio },
                Image = "detail-image-url",
                ThumbnailImage = "thumb-image-url",
            };
        }

        private static Product CreateDefaultProduct()
        {
            return new Product
            {
                Barcode = "barcode",
                Description = "desc",
                Brand = "brand",
                Id = "some-id",
                Name = "prod-name",
                Type = ProductType.Hrana,
                ProductClassifiers = new List<ProductClassifier> { ProductClassifier.Bio },
                DetailImage = "detail-image-url",
                ThumbImage = "thumb-image-url",
            };
        }
    }
}
