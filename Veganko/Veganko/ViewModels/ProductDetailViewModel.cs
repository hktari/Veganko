using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Veganko.Models;

namespace Veganko.ViewModels
{
    public class ProductDetailViewModel : BaseViewModel
    {
        public Product Product { get; set; }

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

        public ProductDetailViewModel(Product product)
        {
            Product = product;
            NewComment = new Comment() { Username = "Test user" };    // TODO: add real user data
        }

        //public static Product Product => new Product
        //{
        //    Name = "Čokoladni namaz",
        //    Rating = 3,
        //    Description = "Kdo pa nima rad nutelle... Še posebej, če je vegan.",
        //    Image = "raspeberry_meringue.jpg",
        //    ProductClassifiers = new ObservableCollection<ProductClassifier>
        //    {
        //        ProductClassifier.VEGAN,
        //        ProductClassifier.GLUTENFREE
        //    },
        //    Comments = new ObservableCollection<Comment>
        //    {
        //        new Comment
        //        {
        //            Username = "BigDick112",
        //            Rating = 4,
        //            DatePosted = DateTime.Now,
        //            Text = "Res ful dobro... Močno priporočam."
        //        },
        //        new Comment
        //        {
        //            Username = "Magda_likesbigdick113",
        //            Rating = 3,
        //            DatePosted = DateTime.Now,
        //            Text = "Sreča je kot metulj."
        //        },
        //        new Comment
        //        {
        //            Username = "Janez_iz_portoroža",
        //            Rating = 2,
        //            DatePosted = DateTime.Now,
        //            Text = "Nima točno takšnega okusa kot nutella :/"
        //        },
        //        new Comment
        //        {
        //            Username = "Ed Sheeran",
        //            Rating = 5,
        //            DatePosted = DateTime.Now,
        //            Text = "Real great stuff ! I should write a song about it..."
        //        },
        //        new Comment
        //        {
        //            Username = "zalathecat",
        //            Rating = 5,
        //            DatePosted = DateTime.Now,
        //            Text = "Čokolada je life. In seveda mačke..."
        //        }
        //    }
        //};
    }
}
