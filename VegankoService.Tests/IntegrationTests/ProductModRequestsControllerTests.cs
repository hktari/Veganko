﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            var pmrs = JsonConvert.DeserializeObject<PagedList<ProductModRequestDTO>>(result.GetJson());
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
            var pmr = JsonConvert.DeserializeObject<ProductModRequestDTO>(result.GetJson());
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
            ProductModRequestDTO pmr = await GetProductModRequest("edit_prod_mod_req_id");
            pmr.UnapprovedProduct.Description = "Banana descriptions";
            pmr.UnapprovedProduct.Name = "Hot chocolate bananas";
            pmr.UnapprovedProduct.Type = "BEVERAGE";
            pmr.UnapprovedProduct.ProductClassifiers = 515;

            var result = await client.PutAsync(
                Util.GetRequestUri($"{Uri}/edit_prod_mod_req_id"),
                pmr.GetStringContent());

            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);

            ProductModRequestDTO updatedPMR = await GetProductModRequest("edit_prod_mod_req_id");

            Assert.Equal(pmr.UnapprovedProduct.Description, updatedPMR.UnapprovedProduct.Description);
            Assert.Equal(pmr.UnapprovedProduct.Name, updatedPMR.UnapprovedProduct.Name);
            Assert.Equal(pmr.UnapprovedProduct.Type, updatedPMR.UnapprovedProduct.Type);
            Assert.Equal(pmr.UnapprovedProduct.ProductClassifiers, updatedPMR.UnapprovedProduct.ProductClassifiers);
        }

        [Fact]
        public async Task Put_ActionNew_ConflictBarcode_ResultsInConflict()
        {
            ProductModRequestDTO pmr = await GetProductModRequest("existing_prod_mod_req_id");
            pmr.UnapprovedProduct.Barcode = "conflicting_barcode";

            var result = await client.PutAsync(
                Util.GetRequestUri($"{Uri}/existing_prod_mod_req_id"),
                pmr.GetStringContent());

            Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
        }

        [Fact]
        public async Task Post_ActionEdit_InvalidModelState_ResultsInBadRequest()
        {
            ProductModRequestDTO pmr = new ProductModRequestDTO
            {
                ExistingProductId = "existing_product_id",
                UnapprovedProduct = new Product
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
            ProductModRequestDTO pmr = new ProductModRequestDTO
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
            ProductModRequestDTO pmr = new ProductModRequestDTO
            {
                ExistingProductId = null,
                UnapprovedProduct = new Product
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
            ProductModRequestDTO pmr = new ProductModRequestDTO
            {
                ExistingProductId = "existing_product_id",
                UnapprovedProduct = new Product
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
            ProductModRequestDTO pmr = new ProductModRequestDTO
            {
                ExistingProductId = null,
                UnapprovedProduct = new Product
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

        [Theory]
        [InlineData(Roles.Admin)]
        [InlineData(Roles.Moderator)]
        [InlineData(Roles.Manager)]
        public async Task Delete_PrivilegedUser_ResultsInOkAndItemRemoved(string role)
        {
            var factory = new CustomWebApplicationFactory<Startup>
            {
                FakeUserRole = role
            };
            var client = factory.CreateClient();

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

            var pmr = JsonConvert.DeserializeObject<ProductModRequestDTO>(response.GetJson());

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
            var factory = new CustomWebApplicationFactory<Startup>
            {
                FakeUserRole = role
            };
            var client = factory.CreateClient();

            var response = await client.PostAsync(
                 Util.GetRequestUri($"{Uri}/new_prod_mod_req_id/image"),
                 CreateMultipartContent());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Approve_ActionAdd_Existing_ResultsInOkAndItemBehindProductGet()
        {
            Util.ReinitializeDbForTests(factory.CreateDbContext());

            ProductModRequestDTO productModReq = await GetProductModRequest("new_prod_mod_req_id");
            var result = await client.PutAsync(
                Util.GetRequestUri($"{Uri}/approve/{productModReq.Id}"),
                productModReq.GetStringContent());

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            Product product = JsonConvert.DeserializeObject<Product>(result.GetJson());
            Assert.True(DoProductsEqual(productModReq.UnapprovedProduct, product));

            result = await client.GetAsync(
                Util.GetRequestUri($"products/{product.Id}"));

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task Approve_ActionEdit_Existing_ResultsInOkAndItemUpdated()
        {
            Util.ReinitializeDbForTests(factory.CreateDbContext());

            ProductModRequestDTO productModReq = await GetProductModRequest("edit_prod_mod_req_id");
            var result = await client.PutAsync(
                Util.GetRequestUri($"{Uri}/approve/{productModReq.Id}"),
                productModReq.GetStringContent());

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            Product product = JsonConvert.DeserializeObject<Product>(result.GetJson());
            Assert.True(DoProductsEqual(productModReq.UnapprovedProduct, product, checkId: false));

            result = await client.GetAsync(
                Util.GetRequestUri($"products/{product.Id}"));

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            // Test equality of item behind GET products/{id}
            product = JsonConvert.DeserializeObject<Product>(result.GetJson());
            Assert.True(DoProductsEqual(productModReq.UnapprovedProduct, product, checkId: false));
        }

        [Fact]
        public async Task Approve_ActionEdit_Missing_ResultsInBadRequestAndErrorResponse()
        {
            Util.ReinitializeDbForTests(factory.CreateDbContext());

            ProductModRequestDTO productModReq = await GetProductModRequest("edit_prod_mod_req_id");

            // Delete the product being edited
            await client.DeleteAsync(Util.GetRequestUri($"products/{productModReq.ExistingProductId}"));

            var result = await client.PutAsync(
                Util.GetRequestUri($"{Uri}/approve/{productModReq.Id}"),
                productModReq.GetStringContent());

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            var err = JsonConvert.DeserializeObject<ValidationProblemDetails>(result.GetJson());
            Assert.NotNull(err);
        }

        [Fact]
        public async Task Reject_Existing_ResultsInOkAndStateSet()
        {
            Util.ReinitializeDbForTests(factory.CreateDbContext());

            ProductModRequestDTO productModReq = await GetProductModRequest("edit_prod_mod_req_id");
            var result = await client.GetAsync(Util.GetRequestUri($"{Uri}/reject/{productModReq.Id}"));

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            productModReq = JsonConvert.DeserializeObject<ProductModRequestDTO>(result.GetJson());
            Assert.Equal(ProductModRequestState.Rejected, productModReq.State);
        }

        private bool DoProductsEqual(Product first, Product second, bool checkId = true)
        {
            return
              (!checkId || first.Id == second.Id) &&
              first.ImageName == second.ImageName &&
              first.Name == second.Name &&
              first.Brand == second.Brand &&
              first.Barcode == second.Barcode &&
              first.Description == second.Description &&
              first.ProductClassifiers == second.ProductClassifiers &&
              first.Type == second.Type;
        }

        private HttpContent CreateMultipartContent()
        {
            byte[] image = new byte[256];

            var content = new MultipartFormDataContent
            {
                { new StreamContent(new MemoryStream(image)), "DetailImage", "DetailImage.jpg" },
                { new StreamContent(new MemoryStream(image)), "ThumbImage", "ThumbImage.jpg" }
            };
            return content;
        }

        private async Task<ProductModRequestDTO> GetProductModRequest(string id)
        {
            return JsonConvert.DeserializeObject<ProductModRequestDTO>(
                            (await client.GetAsync(Util.GetRequestUri($"{Uri}/{id}")))
                            .GetJson());
        }
    }
}