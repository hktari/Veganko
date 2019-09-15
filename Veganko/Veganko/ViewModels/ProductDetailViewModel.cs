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
using Veganko.Models.User;
using Veganko.Other;
using Autofac;

namespace Veganko.ViewModels
{
    public class ProductDetailViewModel : BaseViewModel
    {
        public Command SendCommentCommand => new Command(SendComment);
        public Command AddToFavoritesCommand => new Command(AddToFavorites);

        public Product Product { get; set; }
        public ObservableCollection<CommentViewModel> Comments { get; set; }

        private CommentViewModel newComment;
        public CommentViewModel NewComment
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

        private string profileAvatar;
        public string ProfileAvatar
        {
            get => profileAvatar;
            set => SetProperty(ref profileAvatar, value);
        }

        public UserPublicInfo User => App.IoC.Resolve<IAccountService>().User;

        private Favorite favoriteEntry;

        private IDataStore<Comment> commentDataStore;
        private IDataStore<Favorite> favoriteDataStore;
        private IUserService userService;

        public ProductDetailViewModel(Product product)
        {
            Product = product;
            NewComment = CreateDefaultComment();

            commentDataStore = DependencyService.Get<IDataStore<Comment>>();
            favoriteDataStore = DependencyService.Get<IDataStore<Favorite>>();
            userService = App.IoC.Resolve<IUserService>();
            Comments = new ObservableCollection<CommentViewModel>();
            ProfileAvatar = Images.GetProfileAvatarById(User.AvatarId);
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
                            UserId = App.IoC.Resolve<IAccountService>().User.Id,
                            ProductId = Product.Id
                        });
            }
            await RefreshIsFavorite();
        }

        private async void SendComment(object obj)
        {
            var comment = new Comment
            {
                ProductId = Product.Id,
                Rating = NewComment.Rating,
                Text = NewComment.Text,
                UserId = User.Id,
            };

            await commentDataStore.AddItemAsync(comment);
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
                var allUserIds = items.Distinct(new UserIdComparer()).Select(c => c.UserId);
                var userInfos = await userService.GetByIds(allUserIds);
                foreach (var item in items)
                {
                    var userInfo = userInfos.FirstOrDefault(upi => upi.Id == item.UserId);
                    if (userInfo == null)
                    {
                        Debug.WriteLine("Couldn't match user to comment. comment:{0}, userId:{1}", item.Id, item.UserId);
                        continue;
                    }

                    Comments.Add(
                        new CommentViewModel(item, userInfo));
                }
            }
        }
        
        private CommentViewModel CreateDefaultComment()
        {
            return new CommentViewModel(User)
            {
                Rating = 1,
                Text = ""
            };
        }

        private class CommentDatePostedComparer : IComparer<Comment>
        {
            public int Compare(Comment x, Comment y)
            {
                return x.UtcDatePosted.CompareTo(y.UtcDatePosted) * -1; // descending order
            }
        }

        private class UserIdComparer : IEqualityComparer<Comment>
        {
            public bool Equals(Comment x, Comment y)
            {
                if (x == null || y == null)
                {
                    return false;
                }
                else if (x == null && y == null)
                {
                    return true;
                }

                return x.UserId.Equals(y.UserId);
            }

            public int GetHashCode(Comment obj)
            {
                return obj.UserId.GetHashCode();
            }
        }
    }
}
