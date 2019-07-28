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
            context.Products.Add(product);
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Product Get(string id)
        {
            return context.Products.FirstOrDefault(p => p.Id == id);
        }

        public PagedList<Product> GetAll(int page, int count = 10)
        {
            return new PagedList<Product>();
        }

        public void Update(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
