using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;

[assembly: Xamarin.Forms.Dependency(typeof(Veganko.Services.CommentDataStore))]
namespace Veganko.Services
{
    class CommentDataStore : IDataStore<Comment>
    {
        public async Task<bool> AddItemAsync(Comment item)
        {
            await App.MobileService.GetTable<Comment>().InsertAsync(item);
            return true;
        }

        public Task<bool> DeleteItemAsync(Comment item)
        {
            throw new NotImplementedException();
        }

        public Task<Comment> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Comment>> GetItemsAsync(bool forceRefresh = false)
        {
            return await App.MobileService.GetTable<Comment>().ToEnumerableAsync();
        }

        public Task<bool> UpdateItemAsync(Comment item)
        {
            throw new NotImplementedException();
        }
    }
}
