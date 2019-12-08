using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VegankoService.Controllers;
using VegankoService.Data;
using VegankoService.Models;

namespace VegankoService.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private ProductsController pController;

        [TestInitialize]
        public void Init()
        {
            // TODO: add mock logger ?
            pController = new ProductsController(new MockProductRepository(), null);
        }

        //[TestMethod]
        //public void Create_WithInvalidBase64_ReturnsError()
        //{
        //    var product = new ProductInput
        //    {
        //        Name = "test",
        //        Type = "food",
        //        ProductClassifiers = 0,
        //        ImageBase64Encoded = null
        //    };

        //    ActionResult<Product> actionResult = pController.Post(product);
        //    Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        //}
    }
}
