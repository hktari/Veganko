using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Models;
using VegankoService.Models.Stores;

namespace VegankoService.Data.Stores
{
    public class StoresRepository : IStoresRepository
    {
        private readonly VegankoContext context;

        public StoresRepository(VegankoContext context)
        {
            this.context = context;
        }

        public Task Create(Store store)
        {
            context.Add(store);
            return context.SaveChangesAsync();
        }

        public Task Delete(Store store)
        {
            context.Store.Remove(store);
            return context.SaveChangesAsync();
        }

        public Task<Store> Get(string id)
        {
            return context.Store.FindAsync(id);
        }

        public IEnumerable<Store> GetAll(string storeId)
        {
            return context.Store.Where(store => store.ProductId == storeId);
        }

        public Task Update(Store store)
        {
            context.Store.Update(store);
            return context.SaveChangesAsync();
        }
    }
}
