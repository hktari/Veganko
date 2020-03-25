using Veganko.Services.Http.Errors;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Services.Http;
using Veganko.Services.Products;

namespace Veganko.Services
{
    public class ProductDataStore : IProductService
    {
        private readonly IRestService restService;
        private readonly IProductHelper productHelper;

        public ProductDataStore(IRestService restService, IProductHelper productHelper)
        {
            this.restService = restService;
            this.productHelper = productHelper;
        }

        public async Task<Product> AddAsync(Product item)
        {
            RestRequest request = new RestRequest("products", Method.POST);
            request.AddJsonBody(item);

            var response = await restService.ExecuteAsync(request, throwIfUnsuccessful: false);
            if (response.IsSuccessful)
            {
                var product = JsonConvert.DeserializeObject<Product>(response.Content);
                productHelper.AddImageUrls(product);
                return product;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                var err = JsonConvert.DeserializeObject<RequestConflictError<Product>>(response.Content);
                productHelper.AddImageUrls(err.ConflictingItem);
                throw new ServiceConflictException<Product>(err);
            }
            else
            {
                throw new ServiceException(response);
            }
        }

        public Task DeleteAsync(Product item)
        {
            RestRequest request = new RestRequest($"products/{item.Id}", Method.DELETE);
            return restService.ExecuteAsync(request);
        }

        public async Task<Product> GetAsync(string id)
        {
            RestRequest request = new RestRequest($"products/{id}", Method.GET);

            Product product = await restService.ExecuteAsync<Product>(request);
            productHelper.AddImageUrls(product);

            return product;
        }

        public async Task<PagedList<Product>> AllAsync(int page = 1, int pageSize = 10, bool forceRefresh = false, bool includeUnapproved = false)
        {
            RestRequest request = new RestRequest("products", Method.GET);
            request.AddParameter("page", page, ParameterType.QueryString);
            request.AddParameter("pageSize", pageSize, ParameterType.QueryString);

            var productPage = await restService.ExecuteAsync<PagedList<Product>>(request);
            foreach (var product in productPage.Items)
            {
                productHelper.AddImageUrls(product);
            }

            return productPage;
        }

        public async Task<Product> UpdateAsync(Product item)
        {
            RestRequest request = new RestRequest($"products/{item.Id}", Method.PUT);
            request.AddJsonBody(item);

            Product product = await restService.ExecuteAsync<Product>(request);
            productHelper.AddImageUrls(product);

            return product;
        }

        public async Task<Product> UpdateImagesAsync(Product product, byte[] detailImageData, byte[] thumbImageData)
        {
            RestRequest request = new RestRequest($"products/{product.Id}/image", Method.POST);
            request.AddFile("DetailImage", detailImageData, "DetailImage.jpg");
            request.AddFile("ThumbImage", thumbImageData, "ThumbImage.jpg");

            product = await restService.ExecuteAsync<Product>(request);
            productHelper.AddImageUrls(product);

            return product;
        }

        public Task<IEnumerable<Product>> GetUnapprovedAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }
    }
}
