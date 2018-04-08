using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Veganko.Models;
using Veganko.Services;
using Xamarin.Forms;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Veganko.ViewModels
{
    public class ProductDetailViewModel : BaseViewModel
    {
        public Command SendCommentCommand => new Command(SendComment);
        public Command AddToFavoritesCommand => new Command(AddToFavorites);

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

        bool isFavorite;
        public bool IsFavorite
        {
            get
            {
                return isFavorite;
            }
            set
            {
                SetProperty(ref isFavorite, value);
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

        public async Task RefreshIsFavorite()
        {
            var favorites = await DependencyService.Get<IDataStore<FavoritesEntry>>().GetItemsAsync();
            IsFavorite = favorites.Any(fe => fe.ProductId == Product.Id);
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

        private async void AddToFavorites()
        {
            // TODO: handle IsBusy
            bool success;
            if (IsFavorite)
            {
                success = await DependencyService.Get<IDataStore<FavoritesEntry>>()
                    .DeleteItemAsync(
                        new FavoritesEntry
                        {
                            UserId = DependencyService.Get<IAccountService>().User.Id,
                            ProductId = Product.Id
                        });
            }
            else
            {
                success = await DependencyService.Get<IDataStore<FavoritesEntry>>()
                    .AddItemAsync(
                        new FavoritesEntry
                        {
                            UserId = DependencyService.Get<IAccountService>().User.Id,
                            ProductId = Product.Id
                        });
            }
            if (!success)
                Debug.WriteLine("Couldn't add to favorites !");
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
