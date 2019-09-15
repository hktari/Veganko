using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;

namespace Veganko.Services.Comments
{
    // TODO: implement new interface
	public class MockCommentDataStore : IDataStore<Comment>
	{
        private static int idCtr = 0;
        List<Comment> comments = new List<Comment>();

        public MockCommentDataStore()
        {
            var tmp = new List<Comment>
            {
                new Comment
                {
                    ProductId = "0",
                    UtcDatePosted = DateTime.Now,
                    Rating = 1, Text = "Very good product !",
                    UserId = "0"
                },
                new Comment
                {
                    ProductId = "0",
                    UtcDatePosted = DateTime.Now,
                    Rating = 3, Text = "Idd !",
                    UserId = "0"
                },
                new Comment
                {
                    ProductId = "0",
                    UtcDatePosted = DateTime.Now,
                    Rating = 1, Text = "Boka jedi čaj naret !",
                    UserId = "1"
                },
                new Comment
                {
                    ProductId = "1",
                    Rating = 4,
                    UtcDatePosted = DateTime.Now,
                    Text = "Res ful dobro... Močno priporočam.",
                    UserId = "2",
                },
                new Comment
                {
                    ProductId = "1",
                    UserId = "2",
                    Rating = 3,
                    UtcDatePosted = DateTime.Now,
                    Text = "Sreča je kot metulj."
                },
                new Comment
                {
                    ProductId = "2",
                    UserId = "1",
                    Rating = 2,
                    UtcDatePosted = DateTime.Now,
                    Text = "Nima točno takšnega okusa kot nutella :/"
                },
                new Comment
                {
                    ProductId = "2",
                    UserId = "0",
                    Rating = 5,
                    UtcDatePosted = DateTime.Now,
                    Text = "Real great stuff ! I should write a song about it..."
                },
                new Comment
                {
                    ProductId = "2",
                    UserId = "1",
                    Rating = 5,
                    UtcDatePosted = DateTime.Now,
                    Text = "Čokolada je life. In seveda mačke..."
                }
            };

            foreach (var item in tmp)
                AddItemAsync(item);
    }

        public Task<bool> AddItemAsync(Comment item)
		{
            item.Id = idCtr.ToString();
            idCtr++;
            item.UtcDatePosted = DateTime.Now;
            comments.Add(item);
            return Task.FromResult(true);
        }

		public Task<bool> DeleteItemAsync(Comment item)
		{
            if (comments.Contains(item))
            {
                comments.Remove(item);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

		public Task<Comment> GetItemAsync(string id)
		{
            return Task.FromResult(
                comments.Single(i => i.Id == id));
		}

		public Task<IEnumerable<Comment>> GetItemsAsync(bool forceRefresh = false)
		{
            return Task.FromResult(comments.AsEnumerable());
        }

		public Task<bool> UpdateItemAsync(Comment item)
		{
			throw new NotImplementedException();
		}
	}
}
