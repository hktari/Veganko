using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Veganko.Models;
using Veganko.Services;
using Xamarin.Forms;
using System.Linq;
using System.Threading.Tasks;

namespace Veganko.ViewModels
{
    public class ProductDetailViewModel : BaseViewModel
    {
        public Command SendCommentCommand => new Command(SendComment);

        public Product Product { get; set; }
        public ObservableCollection<Comment> Comments { get; set; }

        private Comment newComment;
        public Comment NewComment
        {
            get
            {
                return newComment;
            }
            set
            {
                SetProperty(ref newComment, value);
            }
        }

        IDataStore<Comment> commentDataStore;

        public ProductDetailViewModel(Product product)
        {
            Product = product;
            NewComment = CreateDefaultComment();

            commentDataStore = DependencyService.Get<IDataStore<Comment>>();
            Comments = new ObservableCollection<Comment>();
        }

        private async void SendComment(object obj)
        {
            await commentDataStore.AddItemAsync(NewComment);
            await RefreshComments();

            NewComment = CreateDefaultComment();
        }

        public async Task RefreshComments()
        {
            Comments.Clear();
            var result = await commentDataStore.GetItemsAsync();
            if (result != null)
            {
                var items = result.Where(c => c.ProductId == Product.Id).ToList();
                items.Sort(new CommentDatePostedComparer());
                foreach (var item in items)
                    Comments.Add(item);
            }
        }
        
        private Comment CreateDefaultComment()
        {
            return new Comment() { Username = "Test user", ProductId = Product.Id, Rating = 1, Text = "" };    // TODO: add real user data
        }

        private class CommentDatePostedComparer : IComparer<Comment>
        {
            public int Compare(Comment x, Comment y)
            {
                return x.DatePosted.CompareTo(y.DatePosted) * -1; // descending order
            }
        }
    }
}
