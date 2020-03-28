using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;
using ProductModRequestDTO = Veganko.Common.Models.Products.ProductModRequestDTO;
using Veganko.Models;
using Veganko.Services.Http;
using Veganko.Services.Http.Errors;
using Veganko.Common.Models.Products;

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
                productHelper.AddImageUrls(pmr.UnapprovedProduct);
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

        public async Task<ProductModRequestDTO> UpdateAsync(ProductModRequestDTO item)
        {
            RestRequest request = new RestRequest($"{Uri}/{item.Id}", Method.PUT);
            request.AddJsonBody(item);

            var response = await restService.ExecuteAsync(request, throwIfUnsuccessful: false);
            if (response.IsSuccessful)
            {
                var pmr = JsonConvert.DeserializeObject<ProductModRequestDTO>(response.Content);
                productHelper.AddImageUrls(pmr.UnapprovedProduct);
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

        public Task DeleteAsync(ProductModRequestDTO item)
        {
            RestRequest request = new RestRequest($"{Uri}/{item.Id}", Method.DELETE);
            return restService.ExecuteAsync(request);
        }

        public Task<ProductModRequestDTO> GetAsync(string id)
        {
            RestRequest request = new RestRequest($"{Uri}/{id}", Method.GET);
            return restService.ExecuteAsync<ProductModRequestDTO>(request);
        }

        public Task<PagedList<ProductModRequestDTO>> AllAsync(int page = 1, int pageSize = 10, string userId = null, ProductModRequestState? state = null, bool forceRefresh = false)
        {
            string url = $"{Uri}?page={page}&pageSize={pageSize}";
            if (userId != null)
            {
                url += $"&userId={userId}";
            }
            
            if (state != null)
            {
                url += $"&state={state}";
            }

            RestRequest request = new RestRequest(url, Method.GET);
            return restService.ExecuteAsync<PagedList<ProductModRequestDTO>>(request);
        }

        public async Task<ProductModRequestDTO> ApproveAsync(ProductModRequestDTO item)
        {
            RestRequest request = new RestRequest($"{Uri}/approve/{item.Id}", Method.PUT);
            request.AddJsonBody(item);

            var response = await restService.ExecuteAsync(request, throwIfUnsuccessful: false);
            if (response.IsSuccessful)
            {
                var pmr = JsonConvert.DeserializeObject<ProductModRequestDTO>(response.Content);
                productHelper.AddImageUrls(pmr.UnapprovedProduct);
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

        public Task<ProductModRequestDTO> RejectAsync(ProductModRequestDTO item)
        {
            RestRequest request = new RestRequest($"{Uri}/reject/{item.Id}", Method.GET);
            return restService.ExecuteAsync<ProductModRequestDTO>(request);
        }

        public async Task<ProductModRequestDTO> UpdateImagesAsync(ProductModRequestDTO item, byte[] detailImageData, byte[] thumbImageData)
        {
            RestRequest request = new RestRequest($"{Uri}/{item.Id}/image", Method.POST);
            request.AddFile("DetailImage", detailImageData, "DetailImage.jpg");
            request.AddFile("ThumbImage", thumbImageData, "ThumbImage.jpg");

            item = await restService.ExecuteAsync<ProductModRequestDTO>(request);
            productHelper.AddImageUrls(item.UnapprovedProduct);
            return item;
        }
    }
}
