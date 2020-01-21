using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Models.Products.Stores;

namespace Veganko.Services.Products.Stores
{
    public interface IStoresService
    {
        Task<Store> Add(Store store);

        Task<Store> Update(Store store);

        Task Remove(Store store);

        Task<IEnumerable<Store>> All(string productId);
    }
}
