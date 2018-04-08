using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Xamarin.Forms;

namespace Veganko.Services
{
    class FavoritesDataStore : IDataStore<FavoritesEntry>
    {
        public async Task<bool> AddItemAsync(FavoritesEntry item)
        {
            await App.MobileService.GetTable<FavoritesEntry>().InsertAsync(item);
            return true;
        }

        public Task<bool> DeleteItemAsync(FavoritesEntry item)
        {
            throw new NotImplementedException();
        }

        public Task<FavoritesEntry> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<FavoritesEntry>> GetItemsAsync(bool forceRefresh = false)
        {
            User user = DependencyService.Get<IAccountService>().User;
            return await App.MobileService.GetTable<FavoritesEntry>().Where(fe => fe.UserId == user.Id).ToListAsync();
        }

        public Task<bool> UpdateItemAsync(FavoritesEntry item)
        {
            throw new NotImplementedException();
        }
    }
}
