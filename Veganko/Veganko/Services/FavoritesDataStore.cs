using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Models.User;
using Xamarin.Forms;

//[assembly: Xamarin.Forms.Dependency(typeof(Veganko.Services.FavoritesDataStore))]
namespace Veganko.Services
{
    class FavoritesDataStore : IDataStore<Favorite>
    {
        public async Task<bool> AddItemAsync(Favorite item)
        {
            await App.MobileService.GetTable<Favorite>().InsertAsync(item);
            return true;
        }

        public async Task<bool> DeleteItemAsync(Favorite item)
        {
            bool success = true;
            try
            {
                await App.MobileService.GetTable<Favorite>().DeleteAsync(item);
            }
            catch
            {
                success = false;
            }
            return await Task.FromResult(success);
        }

        public Task<Favorite> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Favorite>> GetItemsAsync(bool forceRefresh = false)
        {
            User user = App.IoC.Resolve<IAccountService>().User;
            return await App.MobileService.GetTable<Favorite>().Where(fe => fe.UserId == user.Id).ToListAsync();
        }

        public Task<bool> UpdateItemAsync(Favorite item)
        {
            throw new NotImplementedException();
        }
    }
}
