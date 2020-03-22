using Microsoft.AspNetCore.Identity;
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
using VegankoService.Tests.Services;
using Xunit;

namespace VegankoService.Tests.IntegrationTests
{
    public class AccountControllerTests :
        IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;
        private readonly HttpClient client;

        public AccountControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            client = factory.CreateClient();
        }

        #region MockUserManager
        private class MockUserManagerWebAppFactory : CustomWebApplicationFactory<Startup>
        {
            protected override void OnConfigureTestServices(IServiceCollection services)
            {
                base.OnConfigureTestServices(services);
                services.AddScoped<UserManager<ApplicationUser>, MockUserManager>();
            }
        }

        private HttpClient GetMockedUserManagerClient()
        {
            var factory = new MockUserManagerWebAppFactory();
            
            //factory.WithWebHostBuilder(builder => 
            //{
            //    builder.ConfigureTestServices(services => 
            //    {
            //        services.AddScoped<UserManager<ApplicationUser>, MockUserManager>();
            //    });
            //});

            return factory.CreateClient();
        }
        #endregion

        [Fact]
        public async Task CreateAccount_ResultsInOkAndEmailSentAsync()
        {
            var client = GetMockedUserManagerClient();
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
            // Mock not being picked
            var client = GetMockedUserManagerClient();
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

        [Fact]
        public async Task ResendConfirmationEmail_ExistingNonConfirmedUser_ResultsInOkAndEmailSent()
        {
            var client = GetMockedUserManagerClient();
            var result = await client.GetAsync(
                Util.GetRequestUri("account/resend_confirmation_email?email=unconfirmed@email.com"));

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("unconfirmed@email.com", MockEmailService.LastSentToEmail);
        }

        [Fact]
        public async Task ResendConfirmationEmail_NonExistingUser_ResultsInBadRequest()
        {
            var client = GetMockedUserManagerClient();
            var result = await client.GetAsync(
                Util.GetRequestUri("account/resend_confirmation_email?email=nonexisting@email.com"));

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.NotEqual("unconfirmed@email.com", MockEmailService.LastSentToEmail);
        }

        [Theory]
        [InlineData("invalidemail")]
        [InlineData("invalid.com")]
        [InlineData("@e.com")]
        public async Task InvalidEmailAsInput_ResultsInBadRequest(string email)
        {
            var client = GetMockedUserManagerClient();
            var result = await client.GetAsync(
                Util.GetRequestUri($"account/resend_confirmation_email?email={email}"));

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            
            result = await client.PostAsync(
                Util.GetRequestUri("account"),
                new AccountInput
                {
                    Email = email,
                    Username = "testusername",
                    Password = "Test123."
                }.GetStringContent());

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
    }
}
