using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Veganko.Common.Models.Products;
using VegankoService.Models;
using VegankoService.Tests.Helpers;
using Xunit;
using static VegankoService.Helpers.Constants.Strings;

namespace VegankoService.Tests.IntegrationTests
{
    public class ProductModRequestsControllerTests :
        IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;
        private readonly HttpClient client;
        private const string Uri = "product_requests";

        public ProductModRequestsControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            factory.FakeUserRole = VegankoService.Helpers.Constants.Strings.Roles.Member;
            client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ResultsInOkAndNonEmptyListWithProductData()
        {
            var result = await client.GetAsync(Util.GetRequestUri(Uri));

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var pmrs = JsonConvert.DeserializeObject<PagedList<ProductModRequest>>(result.GetJson());
            Assert.True(pmrs.Items.Count() > 0);
            Assert.All(pmrs.Items, pmr => Assert.NotNull(pmr.UnapprovedProduct));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetAll_PageLessThan1_ResultsInBadRequest(int page)
        {
            var result = await client.GetAsync(
                Util.GetRequestUri($"{Uri}?page={page}"));

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Get_ResultsInOkAndValidData()
        {
            var result = await client.GetAsync(
                Util.GetRequestUri($"{Uri}/existing_prod_mod_req_id"));

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var pmr = JsonConvert.DeserializeObject<ProductModRequest>(result.GetJson());
            Assert.NotNull(pmr.UnapprovedProduct);
        }

        [Fact]
        public async Task Get_NonExisting_ResultsInNotFound()
        {
            var result = await client.GetAsync(Util.GetRequestUri($"{Uri}/non_existing"));

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task Put_InvalidModelState_ResultsInBadRequest()
        {
            var pmr = await GetProductModRequest("edit_prod_mod_req_id");

            pmr.ChangedFields = null;

            var result = await client.PutAsync(
                Util.GetRequestUri($"{Uri}/edit_prod_mod_req_id"), pmr.GetStringContent());

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            pmr.ChangedFields = "barcode";
            pmr.UnapprovedProduct = null;

            result = await client.PutAsync(
                Util.GetRequestUri($"{Uri}/edit_prod_mod_req_id"), pmr.GetStringContent());

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Put_EditUnprvdProduct_ResultsInOkAndChangesApplied()
        {
            ProductModRequest pmr = await GetProductModRequest("edit_prod_mod_req_id");
            pmr.UnapprovedProduct.Description = "Banana descriptions";
            pmr.UnapprovedProduct.Name = "Hot chocolate bananas";
            pmr.UnapprovedProduct.Type = "BEVERAGE";
            pmr.UnapprovedProduct.ProductClassifiers = 515;

            var result = await client.PutAsync(
                Util.GetRequestUri($"{Uri}/edit_prod_mod_req_id"),
                pmr.GetStringContent());

            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);

            ProductModRequest updatedPMR = await GetProductModRequest("edit_prod_mod_req_id");

            Assert.Equal(pmr.UnapprovedProduct.Description, updatedPMR.UnapprovedProduct.Description);
            Assert.Equal(pmr.UnapprovedProduct.Name, updatedPMR.UnapprovedProduct.Name);
            Assert.Equal(pmr.UnapprovedProduct.Type, updatedPMR.UnapprovedProduct.Type);
            Assert.Equal(pmr.UnapprovedProduct.ProductClassifiers, updatedPMR.UnapprovedProduct.ProductClassifiers);
        }

        [Fact]
        public async Task Put_ActionNew_ConflictBarcode_ResultsInConflict()
        {
            ProductModRequest pmr = await GetProductModRequest("existing_prod_mod_req_id");
            pmr.UnapprovedProduct.Barcode = "conflicting_barcode";

            var result = await client.PutAsync(
                Util.GetRequestUri($"{Uri}/existing_prod_mod_req_id"),
                pmr.GetStringContent());

            Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
        }

        [Fact]
        public async Task Post_ActionEdit_InvalidModelState_ResultsInBadRequest()
        {
            ProductModRequest pmr = new ProductModRequest
            {
                ExistingProductId = "existing_product_id",
                UnapprovedProduct = new UnapprovedProduct
                {
                    Name = "new product name"
                },
                ChangedFields = null,
            };

            var result = await client.PostAsync(
                Util.GetRequestUri(Uri),
                pmr.GetStringContent());

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Post_ActionNew_InvalidModelState_ResultsInBadRequest()
        {
            ProductModRequest pmr = new ProductModRequest
            {
                ExistingProductId = null,
                UnapprovedProduct = null,
                ChangedFields = null,
            };

            var result = await client.PostAsync(
                Util.GetRequestUri(Uri),
                pmr.GetStringContent());

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Post_ActionNew_ResultsInOk()
        {
            ProductModRequest pmr = new ProductModRequest
            {
                ExistingProductId = null,
                UnapprovedProduct = new UnapprovedProduct
                {
                    Name = "Orange leaf tea",
                    ProductClassifiers = 257,
                },
                ChangedFields = null,
            };

            var result = await client.PostAsync(
                Util.GetRequestUri(Uri),
                pmr.GetStringContent());

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }

        [Fact]
        public async Task Post_ActionEdit_ConflictBarcode_ResultsInConflict()
        {
            ProductModRequest pmr = new ProductModRequest
            {
                ExistingProductId = "existing_product_id",
                UnapprovedProduct = new UnapprovedProduct
                {
                    Barcode = "conflicting_barcode"
                },
                ChangedFields = "barcode",
            };

            var result = await client.PostAsync(
                Util.GetRequestUri(Uri),
                pmr.GetStringContent());

            Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
        }

        [Fact]
        public async Task Post_ActionNew_ConflictBarcode_ResultsInConflict()
        {
            ProductModRequest pmr = new ProductModRequest
            {
                ExistingProductId = null,
                UnapprovedProduct = new UnapprovedProduct
                {
                    Name = "Orange leaf tea",
                    ProductClassifiers = 257,
                    Barcode = "conflicting_barcode",
                },
                ChangedFields = null,
            };

            var result = await client.PostAsync(
                Util.GetRequestUri(Uri),
                pmr.GetStringContent());

            Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
        }

        [Fact]
        public async Task Delete_MemberNotAuthor_ResultsInForbidden()
        {
            Util.ReinitializeDbForTests(factory.CreateDbContext());

            var result = await client.DeleteAsync(
                Util.GetRequestUri($"{Uri}/other_user_prod_mod_req_id"));

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public async Task Delete_MemberWhoIsAuthor_ResultsInOkAndItemRemoved()
        {
            Util.ReinitializeDbForTests(factory.CreateDbContext());

            var result = await client.DeleteAsync(
                Util.GetRequestUri($"{Uri}/new_prod_mod_req_id"));

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            result = await client.GetAsync(
                Util.GetRequestUri($"{Uri}/new_prod_mod_req_id"));

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task PostImages_ResultsInOkAndContent()
        {
            var response = await client.PostAsync(
                Util.GetRequestUri($"{Uri}/existing_prod_mod_req_id/image"),
                CreateMultipartContent());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var pmr = JsonConvert.DeserializeObject<ProductModRequest>(response.GetJson());

            response = await client.GetAsync($"images/detail/{pmr.UnapprovedProduct.ImageName}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            byte[] data = await response.Content.ReadAsByteArrayAsync();
            Assert.True(data.Length > 0);

            response = await client.GetAsync($"images/thumb/{pmr.UnapprovedProduct.ImageName}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            data = await response.Content.ReadAsByteArrayAsync();
            Assert.True(data.Length > 0);
        }

        [Fact]
        public async Task PostImage_UserIsMemberAndNotAuthor_ResultsInForbidden()
        {
            var response = await client.PostAsync(
                Util.GetRequestUri($"{Uri}/other_user_prod_mod_req_id/image"),
                CreateMultipartContent());

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData(Roles.Admin)]
        [InlineData(Roles.Moderator)]
        [InlineData(Roles.Manager)]
        public async Task PostImage_UserHasElevatedRights_ResultsInOk(string role)
        {
            var factory = new CustomWebApplicationFactory<Startup>();
            factory.FakeUserRole = role;
            var client = factory.CreateClient();

            var response = await client.PostAsync(
                 Util.GetRequestUri($"{Uri}/new_prod_mod_req_id/image"),
                 CreateMultipartContent());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Approve_ActionAdd_Existing_ResultsInOkEqualContent()
        {
            Util.ReinitializeDbForTests(factory.CreateDbContext());

            ProductModRequest productModReq = await GetProductModRequest("new_prod_mod_req_id");
            var result = await client.PostAsync(
                Util.GetRequestUri($"{Uri}/approve/{productModReq.Id}"),
                productModReq.GetStringContent());

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            Product product = JsonConvert.DeserializeObject<Product>(result.GetJson());
            Assert.True(productModReq.UnapprovedProduct.Equals(product));
        }

        private HttpContent CreateMultipartContent()
        {
            byte[] image = new byte[256];

            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(new MemoryStream(image)), "DetailImage", "DetailImage.jpg");
            content.Add(new StreamContent(new MemoryStream(image)), "ThumbImage", "ThumbImage.jpg");
            return content;
        }

        // TODO: manager deletes

            // TODO confirm request

            // TODO: adding images
        private async Task<ProductModRequest> GetProductModRequest(string id)
        {
            return JsonConvert.DeserializeObject<ProductModRequest>(
                            (await client.GetAsync(Util.GetRequestUri($"{Uri}/{id}")))
                            .GetJson());
        }
    }
}
