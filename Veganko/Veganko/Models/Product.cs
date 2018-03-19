using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Veganko.Models
{
    /// <summary>
    /// Classifiers to describe the type of the product, food or cosmetics
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProductClassifier
    {
        [EnumMember(Value = "NOT_SET")]
        NOT_SET,
        [EnumMember(Value = "VEGAN")]
        Vegansko,
        [EnumMember(Value = "VEGETARIAN")]
        Vegeterijansko,
        [EnumMember(Value = "GLUTEN_FREE")]
        GlutenFree,
        [EnumMember(Value = "RAW_VEGAN")]
        RawVegan,
        [EnumMember(Value = "PESCETARIAN")]
        Pesketarijansko,
        [EnumMember(Value = "CRUELTY_FREE")]
        CrueltyFree
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProductType
    {
        [EnumMember(Value = "NOT_SET")]
        NOT_SET,
        [EnumMember(Value = "FOOD")]
        Hrana,
        [EnumMember(Value = "BEVERAGE")]
        Pijaca,
        [EnumMember(Value = "COSMETIC")]
        Kozmetika
    }
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Barcode { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public ObservableCollection<ProductClassifier> ProductClassifiers { get; set; }
        public ProductType Type { get; set; }
        public int Rating { get; set; }
        
        public Product()
        {
            Name = Brand = Barcode = Description = "";
            //Comments = new ObservableCollection<Comment>();
        }
    }
}
