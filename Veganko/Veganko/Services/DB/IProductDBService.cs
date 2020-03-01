using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Models.Products;

namespace Veganko.Services.DB
{
    public interface IProductDBService
    {
        Task<IEnumerable<CachedProduct>> GetAllSeenProducts();

        Task SetProductsAsSeen(IEnumerable<Product> products); 
    }
}
