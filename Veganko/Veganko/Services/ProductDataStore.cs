using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Services.Http;
using Xamarin.Forms;
using XamarinImageUploader;

namespace Veganko.Services
{
    public class ProductDataStore : IProductService
    {
        private readonly IRestService restService;
        private const string DetailImageEndpoint = "products/{id}/images/detail/";
        private const string ThumbImageEndpoint = "products/{id}/images/thumb/";

        public ProductDataStore(IRestService restService)
        {
            this.restService = restService;
        }
        public async Task<Product> AddAsync(Product item)
        {
            RestRequest request = new RestRequest("products", Method.POST);
            request.AddJsonBody(item);
            Product product = await restService.ExecuteAsync<Product>(request);
            AddImageUrls(product);

            return product;
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
            AddImageUrls(product);

            return product;
        }

        /// <summary>
        /// Constructs the urls for the detail and thumb images from the image name and endpoint.
        /// </summary>
        private void AddImageUrls(Product product)
        {
            if (product.ImageName == null)
            {
                return;
            }

            string endpoint = RestService.Endpoint;
            if (!endpoint.EndsWith("/"))
            {
                endpoint += "/";
            }

            product.DetailImage = 
                new Uri(
                    new Uri(
                        new Uri(endpoint),
                        DetailImageEndpoint.Replace("{id}", product.Id)),
                    product.ImageName)
                .AbsoluteUri;

            product.ThumbImage = new Uri(
                    new Uri(
                        new Uri(endpoint),
                        ThumbImageEndpoint.Replace("{id}", product.Id)),
                    product.ImageName)
                .AbsoluteUri;
        }

        public async Task<PagedList<Product>> AllAsync(int page = 1, int pageSize = 10, bool forceRefresh = false, bool includeUnapproved = false)
        {
            RestRequest request = new RestRequest("products", Method.GET);
            request.AddParameter("page", page, ParameterType.QueryString);
            request.AddParameter("pageSize", pageSize, ParameterType.QueryString);

            var productPage = await restService.ExecuteAsync<PagedList<Product>>(request);
            foreach (var product in productPage.Items)
            {
                AddImageUrls(product);
            }

            return productPage;
        }
        
        public async Task<Product> UpdateAsync(Product item)
        {
            RestRequest request = new RestRequest($"products/{item.Id}", Method.PUT);
            request.AddJsonBody(item);
            Product product = await restService.ExecuteAsync<Product>(request);
            AddImageUrls(product);

            return product;
        }

        public Task<IEnumerable<Product>> GetUnapprovedAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }
    }
}
