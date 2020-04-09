using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using VegankoService.Models.Comments;
using Xunit;
using VegankoService.Tests.Helpers;
using Newtonsoft.Json;
using Veganko.Common.Models.Users;

namespace VegankoService.Tests.IntegrationTests
{
    public class CommentsControllerTests :
        IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;
        private readonly HttpClient client;
        private const string Uri = "comments";

        public CommentsControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            factory.FakeUserRole = VegankoService.Helpers.Constants.Strings.Roles.Member;
            client = factory.CreateClient();
        }

        [Fact]
        public async Task Post_ResultsInOkAndData()
        {
            var comment = new CommentInput
            {
                ProductId = "existing_product_id",
                Rating = 3,
                Text = "Great comment.",
            };

            var result = await client.PostAsync(Util.GetRequestUri(Uri), comment.GetStringContent());

            Assert.True(result.IsSuccessStatusCode);

            var created = JsonConvert.DeserializeObject<Comment>(result.GetJson());

            Assert.Equal(comment.ProductId, created.ProductId);
            Assert.Equal(comment.Rating, created.Rating);
            Assert.Equal(comment.Text, created.Text);
            Assert.Equal("user_id", created.UserId);
        }
    }
}
