using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Veganko.Services.Http;

namespace UnitTests.Shared.Services.Http
{
    [TestClass]
    public class RequestErrorTests
    {
        [TestMethod]
        public void TestDeserialization()
        {
            string json = @"{
    ""errors"": {
        ""Username"": [
            ""User name 'bkamnik' is already taken.""
        ],
        ""Email"": [
            ""Email 'bkamnik1995@gmail.com' is already taken.""
        ]
    },
    ""type"": null,
    ""title"": ""One or more validation errors occurred."",
    ""status"": 404,
    ""detail"": null,
    ""instance"": null
}".Replace("\n", string.Empty).Replace("\r", string.Empty);

            var reqError = JsonConvert.DeserializeObject<RequestError>(json);
            Assert.AreEqual("User name 'bkamnik' is already taken.", reqError.Errors["Username"][0]);
            Assert.AreEqual("Email 'bkamnik1995@gmail.com' is already taken.", reqError.Errors["Email"][0]);
            Assert.AreEqual(404, reqError.Status);
            Assert.AreEqual("One or more validation errors occurred.", reqError.Title);
        }
    }
}
