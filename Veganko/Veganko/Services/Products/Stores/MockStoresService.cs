using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models.Products.Stores;

namespace Veganko.Services.Products.Stores
{
    public class MockStoresService : IStoresService
    {
        private static int id;
        private List<Store> stores = new List<Store>();

        public MockStoresService()
        {
            stores.AddRange(new[]
            {
                new Store
                {
                    Id = id++.ToString(),
                    Name = "Špar",
                    Address = new Address { FormattedAddress = "Partizanska cesta 22, 3940 Dubrovje, Slovenia" },
                    Price = 3.99,
                },
                new Store
                {
                    Id = id++.ToString(),
                    Name = "Eurospin",
                    Address = new Address { FormattedAddress = "Triplatova cesta 32, 3940 Dubrovje, Slovenia" },
                    Price = 1.99,
                },
                new Store
                {
                    Id = id++.ToString(),
                    Name = "Hofer",
                    Address = new Address { FormattedAddress = "Polehova cesta 5, 2233 Mozire, Slovenia" },
                    Price = 0.99,
                },
            });
        }

        public Task<Store> Add(Store store)
        {
            store.Id = id.ToString();
            id++;
            stores.Add(store);
            return Task.FromResult(store);
        }

        public Task<IEnumerable<Store>> All(string productId)
        {
            return Task.FromResult(stores.Where(s => s.ProductId == productId || s.ProductId == null));
        }

        public Task Remove(Store store)
        {
            throw new NotImplementedException();
        }

        public Task<Store> Update(Store store)
        {
            throw new NotImplementedException();
        }
    }
}
