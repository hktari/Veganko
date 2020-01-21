using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Models;
using VegankoService.Models.Stores;

namespace VegankoService.Data.Stores
{
    public interface IStoresRepository
    {
        IEnumerable<Store> GetAll(string productId);
        Task<Store> Get(string id);
        Task Create(Store store);
        Task Update(Store store);
        Task Delete(Store store);
    }
}
