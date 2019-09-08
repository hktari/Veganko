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

        public ProductDataStore(IRestService restService)
        {
            this.restService = restService;
        }
        public Task<Product> AddAsync(Product item)
        {
            RestRequest request = new RestRequest("products", Method.POST);
            request.AddJsonBody(item);
            return restService.ExecuteAsync<Product>(request);
        }

        public Task DeleteAsync(Product item)
        {
            RestRequest request = new RestRequest($"products/{item.Id}", Method.DELETE);
            return restService.ExecuteAsync(request);
        }

        public Task<Product> GetAsync(string id)
        {
            RestRequest request = new RestRequest($"products/{id}", Method.GET);
            return restService.ExecuteAsync<Product>(request);
        }

        public Task<PagedList<Product>> AllAsync(int page = 1, int pageSize = 10, bool forceRefresh = false, bool includeUnapproved = false)
        {
            // TODO: include unapproved ?

            RestRequest request = new RestRequest("products", Method.GET);
            request.AddParameter("page", page, ParameterType.QueryString);
            request.AddParameter("pageSize", pageSize, ParameterType.QueryString);

            return restService.ExecuteAsync<PagedList<Product>>(request);
        }
        
        public Task<Product> UpdateAsync(Product item)
        {
            RestRequest request = new RestRequest($"products/{item.Id}", Method.PUT);
            request.AddJsonBody(item);
            return restService.ExecuteAsync<Product>(request);
        }

        public Task<IEnumerable<Product>> GetUnapprovedAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }
    }
}
