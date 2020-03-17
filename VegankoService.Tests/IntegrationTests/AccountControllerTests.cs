using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VegankoService.Models.User;
using VegankoService.Services;
using VegankoService.Tests.Helpers;
using Xunit;

namespace VegankoService.Tests.IntegrationTests
{
    public class AccountControllerTests :
        IClassFixture<CustomWebApplicationFactory<TestStartup>>
    {
        private readonly WebApplicationFactory<TestStartup> factory;

        public AccountControllerTests(CustomWebApplicationFactory<TestStartup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task CreateAccount_ResultsInOkAndEmailSentAsync()
        {
            var client = factory.CreateClient();

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
