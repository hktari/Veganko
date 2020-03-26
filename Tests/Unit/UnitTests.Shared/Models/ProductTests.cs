using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Models;
using System.Linq;
using Veganko.Common.Models.Products;

namespace UnitTests.Shared.Models
{
    [TestClass]
    public class ProductTests
    {
        [TestMethod]
        public void TestDeserialization() 
        {
            string json = @"{""id"":""14bbbd61-2f8c-4dfe-b6c4-51291ad88667"",""state"":0,""name"":""MS. 🐈"",""brand"":""ZALAS CUPS"",""barcode"":""barcode"",""imageName"":""a5b1b2b9-2e40-4631-bb06-17619aa957b0.jpg"",""description"":""A relatively cute cup.\\n"",""productClassifiers"":128,""type"":""BEVERAGE"",""rating"":0}"
            .Replace("\n", String.Empty)
            .Replace("\r", string.Empty);

            Product product = JsonConvert.DeserializeObject<Product>(json);

            Assert.AreEqual("14bbbd61-2f8c-4dfe-b6c4-51291ad88667", product.Id);
            Assert.AreEqual("MS. 🐈", product.Name);
            Assert.AreEqual("ZALAS CUPS", product.Brand);
            Assert.AreEqual("barcode", product.Barcode);
            Assert.AreEqual("a5b1b2b9-2e40-4631-bb06-17619aa957b0.jpg", product.ImageName);
            Assert.AreEqual("A relatively cute cup.\\n", product.Description);
            Assert.AreEqual(128, product.ProductClassifiers);
            Assert.AreEqual(ProductType.Pijaca, product.Type);
        }
    }
}
