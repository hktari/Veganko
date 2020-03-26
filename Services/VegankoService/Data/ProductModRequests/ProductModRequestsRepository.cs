using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Veganko.Common.Models.Products;
using VegankoService.Models;

namespace VegankoService.Data.ProductModRequests
{
    public class ProductModRequestsRepository : IProductModRequestsRepository
    {
        private readonly VegankoContext context;

        public ProductModRequestsRepository(VegankoContext context)
        {
            this.context = context;
        }

        public Task Create(ProductModRequest productModRequest)
        {
            context.ProductModRequests.Add(productModRequest);
            return context.SaveChangesAsync();
        }

        public Task Delete(ProductModRequest productModRequest)
        {
            context.ProductModRequests.Remove(productModRequest);
            return context.SaveChangesAsync();

        }

        public Task<ProductModRequest> Get(string id)
        {
            return context.ProductModRequests
                .Include(pmr => pmr.UnapprovedProduct)
                .Include(pmr => pmr.Evaluations)
                .FirstOrDefaultAsync(pmr => pmr.Id == id);
        }

        public PagedList<ProductModRequest> GetAll(string userId, int page, int pageSize = 10)
        {
            return new PagedList<ProductModRequest>
            {
                Items = context.ProductModRequests
                    .Include(pmr => pmr.UnapprovedProduct)
                    .Include(pmr => pmr.Evaluations)
                    .Where(pmr => pmr.UserId == userId),

                Page = page,
                PageSize = pageSize,
                TotalCount = context.ProductModRequests.Count(pmr => pmr.UserId == userId),
            };
        }

        public PagedList<ProductModRequest> GetAll(int page, int pageSize = 10)
        {
            return new PagedList<ProductModRequest>
            {
                Items = context.ProductModRequests
                    .Include(pmr => pmr.UnapprovedProduct)
                    .Include(pmr => pmr.Evaluations),
                
                Page = page,
                PageSize = pageSize,
                TotalCount = context.ProductModRequests.Count(),
            };
        }

        public Task Update(ProductModRequest productModRequest)
        {
            context.ProductModRequests.Update(productModRequest);
            return context.SaveChangesAsync();
        }
    }
}
