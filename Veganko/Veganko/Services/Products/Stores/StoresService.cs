using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models.Products.Stores;
using Veganko.Services.Http;
using RestSharp;

namespace Veganko.Services.Products.Stores
{
    public class StoresService : IStoresService
    {
        private const string StoresUri = "stores";
        private readonly IRestService restService;

        public StoresService(IRestService restService)
        {
            this.restService = restService;
        }

        public Task<Store> Add(Store store)
        {
            RestRequest request = new RestRequest(StoresUri, Method.POST);
            request.AddJsonBody(store);

            return restService.ExecuteAsync<Store>(request);
        }

        public async Task<IEnumerable<Store>> All(string productId)
        {
            RestRequest request = new RestRequest(StoresUri, Method.GET);
            request.AddQueryParameter("productId", productId);

            return await restService.ExecuteAsync<List<Store>>(request);
        }

        public Task Remove(Store store)
        {
            RestRequest request = new RestRequest($"{StoresUri}/{store.Id}", Method.DELETE);
            return restService.ExecuteAsync(request);
        }

        public Task Update(Store store)
        {
            RestRequest request = new RestRequest($"{StoresUri}/{store.Id}", Method.PUT);
            return restService.ExecuteAsync(request);
        }
    }
}
