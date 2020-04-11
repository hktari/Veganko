using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Veganko.Common.Models.Products;
using VegankoService.Models;
using VegankoService.Models.ErrorHandling;

namespace VegankoService.Data
{
    public class MockProductRepository : IProductRepository
    {
        private List<Product> db = new List<Product>();
        private static int productIdxCntr = 0;

        public void Create(Product product)
        {
            product.Id = productIdxCntr++.ToString();
            db.Add(product);
        }

        public void Delete(string id)
        {
            db.Remove(db.Find(p => p.Id == id));
        }

        public Product Get(string id)
        {
            return db.Find(p => p.Id == id);
        }

        public PagedList<Product> GetAll(int page = 1, int pageSize = 10)
        {
            int pageIdx = page - 1;
            if (pageIdx < 0)
            {
                return new PagedList<Product>();
            }

            return new PagedList<Product>
            {
                Items = db.Skip(pageSize * pageIdx).Take(pageSize),
                Page = page,
                PageSize = pageSize,
                TotalCount = db.Count,
            };
        }

        public void Update(Product product)
        {
            throw new NotImplementedException();
        }

        public DuplicateProblemDetails Contains(Product product)
        {
            throw new NotImplementedException();
        }

        public Task CreateUnapproved(UnapprovedProduct product)
        {
            throw new NotImplementedException();
        }

        public Task<UnapprovedProduct> GetUnapproved(string id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUnapproved(UnapprovedProduct product)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUnapproved(UnapprovedProduct product)
        {
            throw new NotImplementedException();
        }

        public DuplicateProblemDetails Contains(UnapprovedProduct product, string existingProductId)
        {
            throw new NotImplementedException();
        }
    }
}
