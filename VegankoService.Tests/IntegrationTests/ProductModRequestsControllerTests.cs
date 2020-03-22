using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Veganko.Common.Models.Products;
using VegankoService.Models;
using VegankoService.Tests.Helpers;
using Xunit;

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
                Util.GetRequestUri($"{Uri}/new_prod_mod_req_id"));

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

            var result = await client.PutAsync(
                Util.GetRequestUri(Uri),
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
            var result = await client.DeleteAsync(
                Util.GetRequestUri($"{Uri}/other_user_prod_mod_req_id"));

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public async Task Delete_MemberWhoIsAuthor_ResultsInOkAndItemRemoved()
        {
            var result = await client.DeleteAsync(
                Util.GetRequestUri($"{Uri}/new_prod_mod_req_id"));

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            result = await client.GetAsync(
                Util.GetRequestUri($"{Uri}/new_prod_mod_req_id"));

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
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
