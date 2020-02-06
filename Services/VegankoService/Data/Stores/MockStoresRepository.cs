using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Models.Stores;

namespace VegankoService.Data.Stores
{
    public class MockStoresRepository : IStoresRepository
    {
        private List<Store> stores = new List<Store>();

        public Task Create(Store store)
        {
            stores.Add(store);
            return Task.CompletedTask;
        }

        public Task Delete(Store store)
        {
            stores.Remove(store);
            return Task.CompletedTask;
        }

        public Task<Store> Get(string id)
        {
            return Task.FromResult(stores.FirstOrDefault(store => store.Id == id));
        }

        public IEnumerable<Store> GetAll(string productId)
        {
            return stores.Where(store => store.ProductId == productId);
        }

        public async Task Update(Store store)
        {
            await Delete(await Get(store.Id));
            stores.Add(store);
        }
    }
}
