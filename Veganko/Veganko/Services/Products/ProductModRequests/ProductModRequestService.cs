using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ProductModRequestDTO = Veganko.Common.Models.Products.ProductModRequestDTO;
using Veganko.Models;
using Veganko.Services.Http;
using Veganko.Services.Http.Errors;

namespace Veganko.Services.Products.ProductModRequests
{
    public class ProductModRequestService : IProductModRequestService
    {
        private readonly IRestService restService;
        private readonly IProductHelper productHelper;
        private const string Uri = "product_requests";

        public ProductModRequestService(IRestService restService, IProductHelper productHelper)
        {
            this.restService = restService;
            this.productHelper = productHelper;
        }

        public async Task<ProductModRequestDTO> AddAsync(ProductModRequestDTO item)
        {
            RestRequest request = new RestRequest(Uri, Method.POST);
            request.AddJsonBody(item);

            var response = await restService.ExecuteAsync(request, throwIfUnsuccessful: false);
            if (response.IsSuccessful)
            {
                var pmr = JsonConvert.DeserializeObject<ProductModRequestDTO>(response.Content);
                productHelper.AddImageUrls(Product.MapToProduct(pmr.UnapprovedProduct));
                return pmr;
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

        public Task<ProductModRequestDTO> UpdateAsync(ProductModRequestDTO item);
        public Task DeleteAsync(ProductModRequestDTO item);
        public Task<ProductModRequestDTO> GetAsync(string id);
        public Task<PagedList<ProductModRequestDTO>> AllAsync(int page = 1, int pageSize = 10, bool forceRefresh = false, bool includeUnapproved = false);
        public Task<IEnumerable<ProductModRequestDTO>> GetUnapprovedAsync(bool forceRefresh = false);
        public Task<ProductModRequestDTO> UpdateImagesAsync(ProductModRequestDTO product, byte[] detailImageData, byte[] thumbImageData);
    }
}
