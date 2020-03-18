using Microsoft.AspNetCore.Mvc;
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
            client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateAccount_ResultsInOkAndEmailSentAsync()
        {
            var response = await client.PostAsync(
                Util.GetRequestUri("account"),
                new AccountInput
                {
                    Username = "test",
                    Email = "test@email.com",
                    Password = "Test123.",
                }.GetStringContent());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("test@email.com", MockEmailService.LastSentToEmail);
        }

        [Fact]
        public async Task CreateAccount_DuplicateUnconfirmedEmail_ResultsInOkAndAccountRecreation() 
        {
            await client.PostAsync(
                 Util.GetRequestUri("account"),
                 new AccountInput
                 {
                     Username = "test",
                     Email = "test@email.com",
                     Password = "Test123.",
                 }.GetStringContent());

            var response = await client.PostAsync(
                Util.GetRequestUri("account"),
                new AccountInput
                {
                    Username = "test2",
                    Email = "test@email.com",
                    Password = "Test222.",
                }.GetStringContent());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await client.GetAsync(
                Util.GetRequestUri("users"));

            var customerPage = JsonConvert.DeserializeObject<PagedList<CustomerProfile>>(response.GetJson());

            Assert.DoesNotContain(customerPage.Items, cp => cp.Username == "test");
            Assert.Contains(customerPage.Items, cp => cp.Username == "test2" && cp.Email == "test@email.com");
        }

        [Fact]
        public async Task CreateAccount_DuplicateEmail_ResultsInBadRequestAndValidationErrors()
        {
            var response = await client.PostAsync(
                Util.GetRequestUri("account"),
                new AccountInput
                {
                    Username = "confirmedUser",
                    Email = "confirmed@email.com",
                    Password = "Test123."
                }.GetStringContent());

            var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(response.GetJson());

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(nameof(AccountInput.Username), problemDetails.Errors.Keys);
            Assert.Contains(nameof(AccountInput.Email), problemDetails.Errors.Keys);
        }
    }
}
