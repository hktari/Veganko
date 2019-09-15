using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Services.Comments;
using Veganko.Services.Http;

namespace Veganko.Services.Comments
{
    public class CommentsService : ICommentsService
    {
        private readonly IRestService restService;

        public CommentsService(IRestService restService)
        {
            this.restService = restService;
        }

        public async Task<Comment> AddItemAsync(Comment item)
        {
            RestRequest request = new RestRequest("comments", Method.POST);
            request.AddJsonBody(item);

            IRestResponse response = await restService.ExecuteAsync(request, throwIfUnsuccessful: false);
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<Comment>(response.Content);
            }
            else
            {
                // TODO: parse error messages
                throw new ServiceException(response);
            }
        }

        public Task DeleteItemAsync(Comment item)
        {
            RestRequest request = new RestRequest($"comments/{item.Id}", Method.DELETE);
            return restService.ExecuteAsync(request);
        }

        public Task<Comment> GetItemAsync(string id)
        {
            RestRequest request = new RestRequest($"comments/{id}", Method.GET);
            return restService.ExecuteAsync<Comment>(request);
        }

        public Task<PagedList<Comment>> GetItemsAsync(string productId, int page = 1, int pageSize = 20, bool forceRefresh = false)
        {
            RestRequest request = new RestRequest("comments", Method.GET);
            request.AddQueryParameter(nameof(productId), productId);
            request.AddQueryParameter(nameof(page), page.ToString());
            request.AddQueryParameter(nameof(pageSize), pageSize.ToString());

            return restService.ExecuteAsync<PagedList<Comment>>(request);
        }

        public async Task<Comment> UpdateItemAsync(Comment item)
        {
            RestRequest request = new RestRequest($"comments/{item.Id}", Method.PUT);
            request.AddJsonBody(item);

            IRestResponse response = await restService.ExecuteAsync(request, throwIfUnsuccessful: false);
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<Comment>(response.Content);
            }
            else
            {
                // TODO: parse error messages
                throw new ServiceException(response);
            }
        }
    }
}
