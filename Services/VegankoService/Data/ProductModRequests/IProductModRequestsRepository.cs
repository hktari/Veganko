using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Veganko.Common.Models.Products;
using VegankoService.Models;

namespace VegankoService.Data.ProductModRequests
{
    public interface IProductModRequestsRepository
    {
        PagedList<ProductModRequest> GetAll(int page, int pageSize = 10);
        Task<ProductModRequest> Get(string id);
        Task Create(ProductModRequest productModRequest);
        Task Update(ProductModRequest productModRequest);
        Task Delete(ProductModRequest productModRequest);
    }
}
