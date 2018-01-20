using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Veganko.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        // TODO: use Comments property from Product class ? 
        // Or filter the products and comments on the server
        // coz there is a database...
        public struct Comment
        {
            public string ProductName { get; set; }
            public int Rating { get; set; }
            public string ImageSrc { get; set; }
            public string Text { get; set; }
        }
        public static ObservableCollection<Comment> Data = new ObservableCollection<Comment>
        {
            new Comment { ProductName = "Vegan Pizza No.2", Rating = 3, ImageSrc = "raspeberry_meringue.jpg", Text = "Well, it's not cheese..." },
            new Comment { ProductName = "Vegan Pizza No.1", Rating = 2, ImageSrc = "raspeberry_meringue.jpg", Text = "Still not cheese..." },
            new Comment { ProductName = "Vanilla Soya Yoghurt", Rating = 5, ImageSrc = "raspeberry_meringue.jpg", Text = "SO GOOD, OH MA GOD !" }
        };
        public ProfileViewModel()
        {
            Title = "Profile";
        }
    }
}
