using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Veganko.Common.Models.Products;
using VegankoService.Models;
using VegankoService.Models.ErrorHandling;

namespace VegankoService.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly VegankoContext context;

        public ProductRepository(VegankoContext context) 
        {
            this.context = context;
        }

        public void Create(Product product)
        {
            product.LastUpdateTimestamp = product.AddedTimestamp = DateTime.Now;
            context.Product.Add(product);
            context.SaveChanges();
        }

        public void Delete(string id)
        {
            context.Product.Remove(
                context.Product.FirstOrDefault(p => p.Id == id) ?? throw new ArgumentException("Can't find product with id:" + id));
            context.SaveChanges();
        }

        public Product Get(string id)
        {
            return context.Product.FirstOrDefault(p => p.Id == id);
        }

        public PagedList<Product> GetAll(int page, int pageSize = 10)
        {
            int pageIdx = page - 1;
            if (pageIdx < 0)
            {
                throw new ArgumentException("Page idx < 0 !");
            }

            return new PagedList<Product>
            {
                Items = context.Product.Skip(pageSize * pageIdx).Take(pageSize),
                Page = page,
                PageSize = pageSize,
                TotalCount = context.Product.Count(),
            };
        }

        public void Update(Product product)
        {
            product.LastUpdateTimestamp = DateTime.Now;
            context.Product.Update(product);
            context.SaveChanges();
        }

        public DuplicateProblemDetails Contains(Product product)
        {
            if (product.Barcode == null)
            {
                return null;
            }

            Product duplicate = context.Product.FirstOrDefault(p => p.Barcode == product.Barcode && p.Id != product.Id);
            if (duplicate != null)
            {
                return new DuplicateProblemDetails(duplicate, "barcode");
            }

            return null;
        }

        public Task CreateUnapproved(Product product)
        {
            product.LastUpdateTimestamp = product.AddedTimestamp = DateTime.Now;
            context.UnapprovedProducts.Add(product);
            return context.SaveChangesAsync();
        }

        public Task<Product> GetUnapproved(string id)
        {
            return context.UnapprovedProducts.FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task DeleteUnapproved(Product product)
        {
            context.UnapprovedProducts.Remove(product);
            return context.SaveChangesAsync();
        }

        public Task UpdateUnapproved(Product product)
        {
            product.LastUpdateTimestamp = DateTime.Now;
            context.UnapprovedProducts.Update(product);
            return context.SaveChangesAsync();
        }
    }
}
