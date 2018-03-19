using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;

[assembly: Xamarin.Forms.Dependency(typeof(Veganko.Services.ProductDataStore))]
namespace Veganko.Services
{
    class ProductDataStore : IDataStore<Product>
    {
        List<Product> products;

        public async Task<bool> AddItemAsync(Product item)
        {
            await App.MobileService.GetTable<Product>().InsertAsync(item);
            return true;
        }

        public async Task<bool> DeleteItemAsync(Product item)
        {
            throw new NotImplementedException();
        }

        public Task<Product> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Product>> GetItemsAsync(bool forceRefresh = false)
        {
            return await App.MobileService.GetTable<Product>().ToEnumerableAsync();
        }

        public Task<bool> UpdateItemAsync(Product item)
        {
            throw new NotImplementedException();
        }
    }
}
