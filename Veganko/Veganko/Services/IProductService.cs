using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;

namespace Veganko.Services
{
    public interface IProductService
    {
        Task<Product> AddAsync(Product item);
        Task<Product> UpdateAsync(Product item);
        Task DeleteAsync(Product item);
        Task<Product> GetAsync(string id);
        Task<PagedList<Product>> AllAsync(int page = 1, int pageSize = 10, bool forceRefresh = false, bool includeUnapproved = false);
        Task<IEnumerable<Product>> GetUnapprovedAsync(bool forceRefresh = false); 
    }
}
