using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Veganko.Models;
using Veganko.Services;
using Xamarin.Forms;

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
            NewComment = new Comment() { Username = "Test user", Rating = 1 };    // TODO: add real user data

            commentDataStore = DependencyService.Get<IDataStore<Comment>>();
            Comments = new ObservableCollection<Comment>();
        }

        private void SendComment(object obj)
        {
            commentDataStore.AddItemAsync(NewComment);
            RefreshComments();
        }

        public void RefreshComments()
        {
            Comments.Clear();
            foreach (var item in commentDataStore.GetItemsAsync().Result)
                Comments.Add(item);
        }
    }
}
