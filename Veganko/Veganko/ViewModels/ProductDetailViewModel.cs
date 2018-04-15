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

        private Favorite favoriteEntry;

        IDataStore<Comment> commentDataStore;
        IDataStore<Favorite> favoriteDataStore;

        public ProductDetailViewModel(Product product)
        {
            Product = product;
            NewComment = CreateDefaultComment();

            commentDataStore = DependencyService.Get<IDataStore<Comment>>();
            favoriteDataStore = DependencyService.Get<IDataStore<Favorite>>();

            Comments = new ObservableCollection<Comment>();
        }

        public async Task RefreshIsFavorite()
        {
            // TODO: GetItem(ID)
            var favorites = await favoriteDataStore.GetItemsAsync();
            favoriteEntry = favorites.FirstOrDefault(fe => fe.ProductId == Product.Id);
            IsFavorite = favoriteEntry != null;
        }

        private async void AddToFavorites()
        {
            // TODO: handle IsBusy
            if (IsFavorite)
            {
                await DependencyService.Get<IDataStore<Favorite>>()
                    .DeleteItemAsync(favoriteEntry);
            }
            else
            {
                await DependencyService.Get<IDataStore<Favorite>>()
                    .AddItemAsync(
                        new Favorite
                        {
                            UserId = DependencyService.Get<IAccountService>().User.Id,
                            ProductId = Product.Id
                        });
            }
            await RefreshIsFavorite();
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
            return new Comment()
            {
                Username = DependencyService.Get<IAccountService>()
                                            .User.Username,
                ProductId = Product.Id,
                Rating = 1,
                Text = ""
            };
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
