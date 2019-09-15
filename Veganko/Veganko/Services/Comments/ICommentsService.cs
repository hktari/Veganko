using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;

namespace Veganko.Services.Comments
{
    public interface ICommentsService
    {
        Task<Comment> AddItemAsync(Comment item);
        Task<Comment> UpdateItemAsync(Comment item);
        Task DeleteItemAsync(string id);
        Task<Comment> GetItemAsync(string id);
        Task<PagedList<Comment>> GetItemsAsync(string productId, int page = 1, int pageSize = 20, bool forceRefresh = false);
    }
}
