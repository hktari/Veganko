using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Veganko.Common.Models.Products;
using VegankoService.Models;
using VegankoService.Models.ErrorHandling;

namespace VegankoService.Data
{
    public interface IProductRepository
    {
        PagedList<Product> GetAll(int page, int count = 10);
        Product Get(string id);
        void Create(Product product);
        void Update(Product product);
        void Delete(string id);
        DuplicateProblemDetails Contains(Product product);
        Task CreateUnapproved(UnapprovedProduct product);
        Task<UnapprovedProduct> GetUnapproved(string id);
        Task DeleteUnapproved(UnapprovedProduct product);
        Task UpdateUnapproved(UnapprovedProduct product);
    }
}
