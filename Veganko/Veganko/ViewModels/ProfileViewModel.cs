using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Services;
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
        public static ObservableCollection<Comment> Data = new ObservableCollection<Comment>
        {
            new Comment { ProductName = "Vegan Pizza No.2", Rating = 3, ImageSrc = "raspeberry_meringue.jpg", Text = "Well, it's not cheese..." },
            new Comment { ProductName = "Vegan Pizza No.1", Rating = 2, ImageSrc = "raspeberry_meringue.jpg", Text = "Still not cheese..." },
            new Comment { ProductName = "Vanilla Soya Yoghurt", Rating = 5, ImageSrc = "raspeberry_meringue.jpg", Text = "SO GOOD, OH MA GOD !" }
        };
        
        public User User { get; set; }

        public ObservableCollection<ProfileComment> Comments = new ObservableCollection<ProfileComment>();

        private IDataStore<Comment> commentDataStore;
        private IDataStore<Product> productDataStore;

        public ProfileViewModel()
        {
            Title = "Profile";
            User = DependencyService.Get<IAccountService>().User;
            commentDataStore = DependencyService.Get<IDataStore<Comment>>();
            productDataStore = DependencyService.Get<IDataStore<Product>>();
        }
        public async Task Refresh()
        {
            var commentData = await commentDataStore.GetItemsAsync();
            var productData = await productDataStore.GetItemsAsync();

            if (commentData != null && productData != null)
            {
                commentData = commentData.Where(c => c.Username == User.Username);
                foreach (var comment in commentData)
                {
                    var product = productData.FirstOrDefault(p => p.Id == comment.ProductId);
                    var profileComment = new ProfileComment
                    {
                        Comment = comment,
                        Product = product ?? new Product()
                    };
                    Comments.Add(profileComment);
                }
            }
        }
    }
}
