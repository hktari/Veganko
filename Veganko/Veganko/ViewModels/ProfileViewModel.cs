using Autofac;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Models.User;
using Veganko.Other;
using Veganko.Services;
using Veganko.ViewModels.Profile;
using Xamarin.Forms;

namespace Veganko.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public class ProfileComment
        {
            public Comment Comment { get; set; }
            public Product Product { get; set; }
        }

        private string backgroundImage;
        public string BackgroundImage
        {
            get => backgroundImage;
            set => SetProperty(ref backgroundImage, value);
        }

        private string userDescription;
        public string UserDescription
        {
            get => userDescription;
            set => SetProperty(ref userDescription, value);
        }

        private string userLabel;
        private string UserLabel
        {
            get => userLabel;
            set => SetProperty(ref userLabel, value);
        }

        private string avatarImage;
        public string AvatarImage
        {
            get => avatarImage;
            set => SetProperty(ref avatarImage, value);
        }

        public Command LoadItemsCommand => new Command(
            async () => await Refresh());

        public UserPublicInfo User { get; set; }

        private ObservableCollection<ProfileComment> comments;
        public ObservableCollection<ProfileComment> Comments
        {
            get { return comments; }
            set { SetProperty(ref comments, value); }
        }

        private bool isDirty;
        public bool IsDirty
        {
            get => isDirty;
            set => SetProperty(ref isDirty, value);
        }

        private IDataStore<Comment> commentDataStore;
        private IDataStore<Product> productDataStore;
        private readonly IAccountService accountService;
        private readonly IUserService userService;

        public ProfileViewModel()
        {
            accountService = App.IoC.Resolve<IAccountService>();
            userService = App.IoC.Resolve<IUserService>();
            Title = "Profile";
            User = accountService.User;
            HandleNewData();
            // TODO: fix memory leak
            MessagingCenter.Subscribe<BackgroundImageViewModel, string>(this, BackgroundImageViewModel.SaveMsg, OnBackgroundImageChanged);
            MessagingCenter.Subscribe<SelectAvatarViewModel, string>(this, SelectAvatarViewModel.SaveMsg, OnAvatarImageChanged);
            Comments = new ObservableCollection<ProfileComment>();
            commentDataStore = DependencyService.Get<IDataStore<Comment>>();
            productDataStore = DependencyService.Get<IDataStore<Product>>();
        }

        public async Task SaveProfile()
        {
            // Update the in memory cache of the user model
            accountService.User = await userService.Edit(
                new UserPublicInfo
                {
                    Label = UserLabel,
                    Description = UserDescription,
                    AvatarId = Images.GetProfileAvatarId(AvatarImage),
                    ProfileBackgroundId = Images.GetProfileBackgroundImageId(BackgroundImage),
                });
        }

        private void HandleNewData()
        {
            UserDescription = User.Description;
            UserLabel = User.Label;
            AvatarImage = Images.GetProfileAvatarById(User.AvatarId);
            BackgroundImage = Images.GetProfileBackgroundImageById(User.ProfileBackgroundId);
        }

        public async Task Refresh()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            try
            {
                var commentData = await commentDataStore.GetItemsAsync();
                var productData = await productDataStore.GetItemsAsync();

                if (commentData != null && productData != null)
                {
                    commentData = commentData.Where(c => c.UserId == User.Id);
                    foreach (var comment in commentData)
                    {
                        var product = productData.FirstOrDefault(p => p.Id == comment.ProductId);
                        // If for some reason the corresponding product isn't found, just ignore the comment
                        if (product == null)
                        {
                            Console.WriteLine($"Ignoring comment {comment.Id} " +
                                $"because the corresponding product {product.Id} couldn't be found.");
                            continue;
                        }
                        var profileComment = new ProfileComment
                        {
                            Comment = comment,
                            Product = product
                        };
                        Comments.Add(profileComment);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not refresh profile comments: " + e.Message + " " + e.StackTrace);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnBackgroundImageChanged(BackgroundImageViewModel arg1, string newBackgroundId)
        {
            BackgroundImage = Images.GetProfileBackgroundImageById(newBackgroundId);
            UpdateIsDirty();
        }

        private void UpdateIsDirty()
        {
            UserPublicInfo user = accountService.User;
            IsDirty =
                Images.GetProfileBackgroundImageId(BackgroundImage) != user.ProfileBackgroundId ||
                Images.GetProfileAvatarId(AvatarImage) != user.AvatarId ||
                UserDescription != user.Description ||
                UserLabel != user.Label;
        }

        private void OnAvatarImageChanged(SelectAvatarViewModel sender, string newAvatarId)
        {
            AvatarImage = Images.GetProfileAvatarById(newAvatarId);
            UpdateIsDirty();
        }
    }
}
