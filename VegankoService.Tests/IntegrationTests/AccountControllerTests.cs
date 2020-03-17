using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VegankoService.Models;
using VegankoService.Models.User;
using VegankoService.Services;
using VegankoService.Tests.Helpers;
using Xunit;

namespace VegankoService.Tests.IntegrationTests
{
    public class AccountControllerTests :
        IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> factory;
        private readonly HttpClient client;

        public AccountControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<IEmailService, MockEmailService>();
                });
            }).CreateClient();
        }

        [Fact]
        public async Task CreateAccount_ResultsInOkAndEmailSentAsync()
        {
            var response = await client.PostAsync(
                Utilities.GetRequestUri("account"),
                new AccountInput
                {
                    Username = "test",
                    Email = "test@email.com",
                    Password = "Test123.",
                }.GetStringContent());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("test@email.com", MockEmailService.LastSentToEmail);
        }
    }
}
