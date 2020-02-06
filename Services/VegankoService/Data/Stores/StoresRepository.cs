using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Models.Stores;
using Microsoft.EntityFrameworkCore;

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
            context.Store.Add(store);
            return context.SaveChangesAsync();
        }

        public Task Delete(Store store)
        {
            context.Store.Remove(store);
            return context.SaveChangesAsync();
        }

        public Task<Store> Get(string id)
        {
            return context.Store
                .Include(store => store.Address)
                .Include(store => store.Coordinates)
                .FirstAsync(store => store.Id == id);
        }

        public IEnumerable<Store> GetAll(string storeId)
        {
            return context.Store
                .Include(store => store.Address)
                .Include(store => store.Coordinates)
                .Where(store => store.ProductId == storeId);
        }

        public Task Update(Store store)
        {
            context.Store.Update(store);
            return context.SaveChangesAsync();
        }
    }
}
