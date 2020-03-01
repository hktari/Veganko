using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Models.Products;

namespace Veganko.Services.DB
{
    public class MockProductDBService : IProductDBService
    {
        public List<CachedProduct> Products { get; set; } = new List<CachedProduct>();

        public Task<IEnumerable<CachedProduct>> GetAllSeenProducts()
        {
            return Task.FromResult(Products.AsEnumerable());
        }

        public Task SetProductsAsSeen(IEnumerable<Product> products)
        {
            throw new NotImplementedException();
        }
    }
}
