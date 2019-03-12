using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;

[assembly: Xamarin.Forms.Dependency(typeof(Veganko.Services.MockFavoriteDataStore))]
namespace Veganko.Services
{
    public class MockFavoriteDataStore : IDataStore<Favorite>
    {
        private readonly List<Favorite> cache = new List<Favorite>()
        {
            new Favorite
            {
                Id = "0",
                ProductId = "0",
                UserId = "0"
            },
            new Favorite
            {
                Id = "1",
                ProductId = "1",
                UserId = "0"
            },
            new Favorite
            {
                Id = "2",
                ProductId = "2",
                UserId = "0"
            },
        };

        public Task<bool> AddItemAsync(Favorite item)
        {
            cache.Add(item);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteItemAsync(Favorite item)
        {
            var idx = cache.IndexOf(item);
            if (idx == -1)
                return Task.FromResult(false);
            cache.RemoveAt(idx);
            return Task.FromResult(true);
        }

        public Task<Favorite> GetItemAsync(string id)
        {
            return Task.FromResult(
                cache.FirstOrDefault(f => f.ProductId == id));
        }

        public Task<IEnumerable<Favorite>> GetItemsAsync(bool forceRefresh = false)
        {
            return Task.FromResult(cache.AsEnumerable());
        }

        public Task<bool> UpdateItemAsync(Favorite item)
        {
            throw new NotImplementedException();
        }
    }
}
