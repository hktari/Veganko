using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;

namespace Veganko.Services
{
    public interface IProductService
    {
        Task<bool> AddAsync(Product item);
        Task<bool> UpdateAsync(Product item);
        Task<bool> DeleteAsync(Product item);
        Task<Product> GetAsync(string id);
        Task<IEnumerable<Product>> AllAsync(bool forceRefresh = false, bool includeUnapproved = false);
        Task<IEnumerable<Product>> GetUnapprovedAsync(bool forceRefresh = false); 
    }
}
