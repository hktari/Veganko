using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Veganko.Models;
using Veganko.Services;
using Xamarin.Forms;
using System.Linq;

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

        private void SendComment(object obj)
        {
            commentDataStore.AddItemAsync(NewComment);
            RefreshComments();

            NewComment = CreateDefaultComment();
        }

        public void RefreshComments()
        {
            Comments.Clear();
            var items = commentDataStore.GetItemsAsync().Result.ToList();
            items.Sort(new CommentDatePostedComparer());
            foreach (var item in items)
                Comments.Add(item);
        }
        
        private Comment CreateDefaultComment()
        {
            return new Comment() { Username = "Test user", Rating = 1, Text = "" };    // TODO: add real user data
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
