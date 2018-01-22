using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace Veganko.Models
{
    /// <summary>
    /// Classifiers to describe the type of the product, food or cosmetics
    /// </summary>
    public enum ProductClassifier
    {
        NOT_SET,
        VEGAN,
        VEGETARIAN,
        GLUTENFREE,
        CRUELTYFREE
    }

    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ImageSource Image { get; set; }
        public string Description { get; set; }
        public ObservableCollection<ProductClassifier> ProductClassifiers { get; set; }
        public int Rating { get; set; }
        public ObservableCollection<Comment> Comments { get; set; }
    }
}
