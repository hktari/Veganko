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
using Veganko.Services.Comments;
using Veganko.Views.Product.DTS;
using Veganko.Views.Product;
using Veganko.Extensions;
using Veganko.Services.Http;
using Veganko.ViewModels.Products.Partial;
using Veganko.Services.Logging;
using Veganko.ViewModels.Stores;
using Veganko.Views.Stores;
using Veganko.Common.Models.Products;
using Veganko.Common.Models.Users;

namespace Veganko.ViewModels.Products
{
    public class ProductDetailViewModel : BaseViewModel
    {
        public Command AddToFavoritesCommand => new Command(AddToFavorites);

        public Command EditCommand => new Command(
            async () =>
            {
                await App.Navigation.PushModalAsync(
                    new NavigationPage(
                        new EditProductPage(new EditProductViewModel(Product))));
            });

        private ProductViewModel product;
        public ProductViewModel Product
        {
            get
            {
                return product;
            }
            set
            {
                SetProperty(ref product, value);
            }
        }

        private ObservableCollection<CommentViewModel> comments;
        public ObservableCollection<CommentViewModel> Comments
        {
            get => comments;
            set => SetProperty(ref comments, value);
        }

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

        private int productRating;
        public int ProductRating
        {
            get
            {
                return productRating;
            }
            set
            {
                SetProperty(ref productRating, value);
            }
        }

        //public CommentDataTemplateSelector CommentDTS { get; set; }

        public UserPublicInfo User => App.IoC.Resolve<IUserService>().CurrentUser;

        public bool IsUserManager { get; set; }

        private Favorite favoriteEntry;

        private ICommentsService commentDataStore;
        private ILogger logger;
        private IDataStore<Favorite> favoriteDataStore;
        private IUserService userService;

        public ProductDetailViewModel(Product product)
            : this(new ProductViewModel(product))
        {
        }

        public ProductDetailViewModel(ProductViewModel product)
        {
            Product = product;
            NewComment = CreateDefaultComment();

            commentDataStore = App.IoC.Resolve<ICommentsService>();
            logger = App.IoC.Resolve<ILogger>();
            favoriteDataStore = DependencyService.Get<IDataStore<Favorite>>();
            userService = App.IoC.Resolve<IUserService>();
            Comments = new ObservableCollection<CommentViewModel>();
            ProfileAvatar = Images.GetProfileAvatarById(User.AvatarId);

            IsUserManager = User.IsManager();
        }

        public Task Init()
        {
            MessagingCenter.Unsubscribe<EditProductViewModel, ProductViewModel>(this, EditProductViewModel.ProductUpdatedMsg);
            MessagingCenter.Subscribe<EditProductViewModel, ProductViewModel>(this, EditProductViewModel.ProductUpdatedMsg, OnProductUpdated);

            if (comments.Count == 0)
            {
                return RefreshComments();
            }

            return Task.CompletedTask;
        }

        public bool CanAddStores => true;

        public Command WhereToBuyCommand => new Command(
            () => 
            {
                App.Navigation.PushAsync(new StoreListPage(new StoreListViewModel(Product.Id)));
            });

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
                            UserId = App.IoC.Resolve<IUserService>().CurrentUser.Id,
                            ProductId = Product.Id
                        });
            }
            await RefreshIsFavorite();
        }

        public async Task DeleteComment(CommentViewModel commentVM)
        {
            await commentDataStore.DeleteItemAsync(commentVM.Id);
            Comments.Remove(commentVM);
        }

        public Command SendCommentCommand => new Command(
            async () =>
            {
                if (string.IsNullOrWhiteSpace(NewComment.Text))
                {
                    await App.CurrentPage.Err("Prosim vnesi text.");
                    return;
                }

                try
                {
                    IsBusy = true; 
                    var comment = new Comment
                    {
                        ProductId = Product.Id,
                        Text = NewComment.Text,
                        UserId = User.Id,
                    };
                    
                    // 0 means no rating was added. This translates to null for backend.
                    if (NewComment.Rating != 0)
                    {
                        comment.Rating = NewComment.Rating;
                    }

                    await commentDataStore.AddItemAsync(comment);
                    await RefreshComments();

                    NewComment = CreateDefaultComment();
                }
                catch (ServiceException ex)
                {
                    logger.LogException(ex);
                    await App.CurrentPage.Err("komentar ni bil poslan");
                }
                finally
                {
                    IsBusy = false;
                }
           });

        public async Task RefreshComments()
        {
            IsBusy = true;
            try
            {
                // Get all comments
                PagedList<Comment> commentsList = await commentDataStore.GetItemsAsync(Product.Id, pageSize: 1000);
                List<Comment> comments = commentsList.Items.ToList();
                comments.Sort(new CommentDatePostedComparer());
                ProductRating = CalculateAvgRating(comments);
                Comments = new ObservableCollection<CommentViewModel>(
                    comments.Select(c => new CommentViewModel(c)));
            }
            catch (ServiceException ex)
            {
                await App.CurrentPage.Err(ex.StatusCodeDescription);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private int CalculateAvgRating(List<Comment> comments)
        {
            // Prevent division by 0
            if (comments.Count == 0)
            {
                return 0;
            }
            
            int allRatings = 0;
            double ratingsSummed = 0;
            
            foreach (var comment in comments)
            {
                if (comment.Rating != null)
                {
                    allRatings++;
                    ratingsSummed += comment.Rating.Value;
                }
            }

            return (int)(ratingsSummed / allRatings);
        }

        private CommentViewModel CreateDefaultComment()
        {
            return new CommentViewModel(User)
            {
                Rating = 0,
                Text = "",
            };
        }

        private void OnProductUpdated(EditProductViewModel sender, ProductViewModel updatedProductViewModel)
        {
            Product.Update(updatedProductViewModel);
        }

        #region Comparers
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
        #endregion
    }
}
