using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Common.Models.Users;
using Veganko.Services.Http;
using Veganko.Services.Storage;
using Veganko.Services.Users;

namespace UnitTests.Shared.Services
{
    [TestClass]
    public class UserServicesTests
    {

        class MockRestService : IRestService
        {
            public Task<IRestResponse> ExecuteAsync(RestRequest request, bool authorize = true, bool throwIfUnsuccessful = true)
            {
                throw new NotImplementedException();
                //return new RestResponse { }
            }

            public Task<TModel> ExecuteAsync<TModel>(RestRequest request, bool authorize = true) where TModel : new()
            {
                object user = new UserPublicInfo
                {
                    Id = "user_id",
                    Username = "test user",
                    Role = UserRole.Manager,
                };

                return Task.FromResult((TModel)user);
            }
        }
        [TestMethod]
        public async Task Expect_EnsureCurrentUserIsSet_ToReloadUserData()
        {
            MockPreferences cache = new MockPreferences();
            UserService userService = new UserService(new MockRestService(), cache);
            userService.SetCurrentUser(new UserPublicInfo
            {
                Id = "user_id",
                Username = "test user",
                Role = UserRole.Member,
            });
            // Simulate restarting the app
            userService = new UserService(new MockRestService(), cache);

            await userService.EnsureCurrentUserIsSet();

            Assert.AreEqual(UserRole.Manager, userService.CurrentUser.Role);
        }
    }
}
