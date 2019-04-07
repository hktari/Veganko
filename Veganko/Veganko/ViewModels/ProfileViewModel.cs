using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Models.User;
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

        private ProfileBackgroundImage backgroundImage;
        public ProfileBackgroundImage BackgroundImage
        {
            get => backgroundImage;
            set => SetProperty(ref backgroundImage, value);
        }

        public Command LoadItemsCommand => new Command(
            async () => await Refresh());

        public User User => DependencyService.Get<IAccountService>().User;

        private ObservableCollection<ProfileComment> comments;
        public ObservableCollection<ProfileComment> Comments
        {
            get { return comments; }
            set { SetProperty(ref comments, value); }
        }
        
        private IDataStore<Comment> commentDataStore;
        private IDataStore<Product> productDataStore;

        public ProfileViewModel()
        {
            Title = "Profile";
            // TODO: fix memory leak
            MessagingCenter.Subscribe<BackgroundImageViewModel, ProfileBackgroundImage>(this, BackgroundImageViewModel.SaveMsg, OnBackgroundImageChanged);
            Comments = new ObservableCollection<ProfileComment>();
            commentDataStore = DependencyService.Get<IDataStore<Comment>>();
            productDataStore = DependencyService.Get<IDataStore<Product>>();
            BackgroundImage = User.ProfileBackground;
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
                    commentData = commentData.Where(c => c.Username == User.Username);
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

        private void OnBackgroundImageChanged(BackgroundImageViewModel arg1, ProfileBackgroundImage selectedBackgroundImg)
        {
            // TODO: service call ?

            User.ProfileBackground = selectedBackgroundImg;
            BackgroundImage = selectedBackgroundImg;
        }
    }
}
