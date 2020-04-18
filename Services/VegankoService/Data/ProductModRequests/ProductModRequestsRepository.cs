using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Veganko.Common.Models.Products;
using VegankoService.Data.Users;
using VegankoService.Models;

namespace VegankoService.Data.ProductModRequests
{
    public class ProductModRequestsRepository : IProductModRequestsRepository
    {
        private readonly VegankoContext context;
        private readonly IUsersRepository usersRepository;

        public ProductModRequestsRepository(VegankoContext context, IUsersRepository usersRepository)
        {
            this.context = context;
            this.usersRepository = usersRepository;
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

        public async Task<ProductModRequest> Get(string id)
        {
            ProductModRequest prodModReq = await context.ProductModRequests
                .Include(pmr => pmr.UnapprovedProduct)
                .Include(pmr => pmr.Evaluations)
                .FirstOrDefaultAsync(pmr => pmr.Id == id);

            return prodModReq;
        }

        public PagedList<ProductModRequest> GetAll(ProductModReqQuery query)
        {
            IQueryable<ProductModRequest> items = context.ProductModRequests
                .Include(pmr => pmr.UnapprovedProduct)
                .Include(pmr => pmr.Evaluations);

            if (query.UserId != null)
            {
                items = items.Where(pmr => pmr.UserId == query.UserId);
            }
            
            if (query.State != null)
            {
                items = items.Where(pmr => pmr.State == query.State);
            }

            int pageModifier = Math.Max(0, query.Page - 1); // Min value is 0

            // Order by newest first
            items = items.OrderByDescending(pmr => pmr.Timestamp);

            return new PagedList<ProductModRequest>
            {
                Items = items.Skip(pageModifier * query.PageSize).Take(query.PageSize),
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = items.Count(),
            };
        }

        public Task Update(ProductModRequest productModRequest)
        {
            productModRequest.Timestamp = DateTime.Now;
            context.ProductModRequests.Update(productModRequest);
            return context.SaveChangesAsync();
        }
    }
}
