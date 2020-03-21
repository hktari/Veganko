using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Veganko.Common.Models.Products;
using VegankoService.Models;
using VegankoService.Tests.Helpers;
using Xunit;

namespace VegankoService.Tests.IntegrationTests
{
    public class ProductControllerTests :
        IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public ProductControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOkPagedList()
        {
            var client = _factory.CreateClient();

            var result = await client.GetAsync(
                Util.GetRequestUri("products?page=1&pageSize=5"));

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var productPage = JsonConvert.DeserializeObject<PagedList<Product>>(
                result.GetJson());

            Assert.Equal(5, productPage.Items.Count());
            Assert.Equal(5, productPage.PageSize);
            Assert.Equal(1, productPage.Page);
            Assert.Equal(9, productPage.TotalCount);
            Assert.Equal(2, productPage.TotalPages);
        }
    }
}
