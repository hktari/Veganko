using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;

namespace Veganko.Services.Comments
{
	public class MockCommentsService : ICommentsService
	{
        private static int idCtr = 0;
        List<Comment> comments = new List<Comment>();

        public MockCommentsService()
        {
            var tmp = new List<Comment>
            {
                new Comment
                {
                    ProductId = "0",
                    UtcDatePosted = DateTime.Now,
                    Rating = 1, Text = "Very good product !",
                    UserId = "0",
                    UserAvatarId = "0",
                    UserProfileBackgroundId = "0",
                },
                new Comment
                {
                    ProductId = "0",
                    UtcDatePosted = DateTime.Now,
                    Rating = 3, Text = "Idd !",
                    UserId = "0",
                    UserAvatarId = "0",
                    UserProfileBackgroundId = "0",
                },
                new Comment
                {
                    ProductId = "0",
                    UtcDatePosted = DateTime.Now,
                    Rating = 1, Text = "Boka jedi čaj naret !",
                    UserId = "1",
                    UserAvatarId = "0",
                    UserProfileBackgroundId = "0",
                },
                new Comment
                {
                    ProductId = "1",
                    Rating = 4,
                    UtcDatePosted = DateTime.Now,
                    Text = "Res ful dobro... Močno priporočam.",
                    UserAvatarId = "0",
                    UserProfileBackgroundId = "0",
                    UserId = "2",
                },
                new Comment
                {
                    ProductId = "1",
                    UserId = "2",
                    Rating = 3,
                    UtcDatePosted = DateTime.Now,
                    UserAvatarId = "0",
                    UserProfileBackgroundId = "0",
                    Text = "Sreča je kot metulj."
                },
                new Comment
                {
                    ProductId = "2",
                    UserId = "1",
                    Rating = 2,
                    UtcDatePosted = DateTime.Now,
                    UserAvatarId = "0",
                    UserProfileBackgroundId = "0",
                    Text = "Nima točno takšnega okusa kot nutella :/"
                },
                new Comment
                {
                    ProductId = "2",
                    UserId = "0",
                    Rating = 5,
                    UtcDatePosted = DateTime.Now,
                    UserAvatarId = "0",
                    UserProfileBackgroundId = "0",
                    Text = "Real great stuff ! I should write a song about it..."
                },
                new Comment
                {
                    ProductId = "2",
                    UserId = "1",
                    Rating = 5,
                    UtcDatePosted = DateTime.Now,
                    UserAvatarId = "0",
                    UserProfileBackgroundId = "0",
                    Text = "Čokolada je life. In seveda mačke..."
                }
            };

            foreach (var item in tmp)
                AddItemAsync(item);
    }

        public Task DeleteItemAsync(string id)
        {
            Comment comment = comments.FirstOrDefault(i => i.Id == id);
            if (comment != null)
            {
                comments.Remove(comment);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<Comment> GetItemAsync(string id)
		{
            return Task.FromResult(
                comments.Single(i => i.Id == id));
		}

        public Task<PagedList<Comment>> GetItemsAsync(string productId, int page = 1, int pageSize = 20, bool forceRefresh = false)
        {
            return Task.FromResult(new PagedList<Comment> {  Items = comments.AsEnumerable() });
        }

        public Task<Comment> AddItemAsync(Comment item)
        {
            item.Id = idCtr.ToString();
            idCtr++;
            item.UtcDatePosted = DateTime.Now;
            comments.Add(item);
            return Task.FromResult(item);
        }

        public Task<Comment> UpdateItemAsync(Comment item)
        {
            throw new NotImplementedException();
        }
    }
}
