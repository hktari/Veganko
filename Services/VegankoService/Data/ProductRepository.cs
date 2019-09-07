using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Models;

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
            context.Product.Update(product);
            context.SaveChanges();
        }
    }
}
