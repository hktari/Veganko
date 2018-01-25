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
        RAW_VEGAN,
        PESCETARIAN,
        CRUELTYFREE
    }
    public enum ProductType
    {
        NOT_SET,
        FOOD, 
        BEVERAGE,
        COSMETICS
    }
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Barcode { get; set; }
        public ImageSource Image { get; set; }
        public string Description { get; set; }
        public ObservableCollection<ProductClassifier> ProductClassifiers { get; set; }
        public ProductType Type { get; set; }
        public int Rating { get; set; }
        public ObservableCollection<Comment> Comments { get; set; }
    }
}
