using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VegankoService.Models;

namespace VegankoService.Tests.IntegrationTests
{
    [TestClass]
    public class VegankoApiTests
    {
        private static HttpClient _client;
        //private string _nonAdminToken;
        //private string _adminToken;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            //const string issuer = "https://localhost:44391";
            //const string key = "some-long-secret-key";

            var server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>().UseUrls("https://localhost:44392"))
                //.UseSetting("Tokens:Issuer", issuer)
                //.UseSetting("Tokens:Key", key))
            {
                BaseAddress = new Uri("https://localhost:44392")
            };

            _client = server.CreateClient();

            //_nonAdminToken = JwtTokenGenerator.Generate(
            //    "aspnetcore-api-demo", false, issuer, key);
            //_adminToken = JwtTokenGenerator.Generate(
            //    "aspnetcore-api-demo", true, issuer, key);
        }

        [TestMethod]
        public async Task CreateProduct_InvalidBase64_ReturnsBadRequest()
        {
            _client.DefaultRequestHeaders.Clear();
            //_client.DefaultRequestHeaders
            //    .Add("Authorization", new[] { $"Bearer {_adminToken}" });

            ProductInput newProduct = new ProductInput { Name = "test", Type = "food", ProductClassifiers = 0, ImageData = null };
            var result = await _client.PostAsJsonAsync("/api/products", newProduct);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public async Task GetAllComments_NonNullProductId_ReturnsOk()
        {
            var result = await _client.GetAsync("/api/comments?productId=some-id");

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
