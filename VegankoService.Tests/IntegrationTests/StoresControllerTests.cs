using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VegankoService.Models.Stores;
using VegankoService.Tests.Helpers;
using Xunit;

namespace VegankoService.Tests.IntegrationTests
{
    public class StoresControllerTests :
        IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;
        private readonly HttpClient client;
        private const string Uri = "stores";

        public StoresControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            factory.FakeUserRole = VegankoService.Helpers.Constants.Strings.Roles.Member;
            client = factory.CreateClient();
        }

        [Fact]
        public async Task Post_UnapprovedProduct_ResultsInOkAsync()
        {
            var result = await client.PostAsync(Util.GetRequestUri(Uri), new Store
            {
                ProductId = "new_unprvd_product",
                Address = new Address { FormattedAddress = "address" },
                Name = "Store name",
                Price = 2.99,
            }.GetStringContent());

            Assert.True(result.IsSuccessStatusCode);
        }
    }
}
