using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Veganko.Common.Models.Products
{
    /// <summary>
    /// Classifiers to describe the type of the product, food or cosmetics
    /// </summary>
    public enum ProductClassifier
    {
        NOT_SET = 1,
        Vegansko = 2,
        Vegeterijansko = 4,
        GlutenFree = 8,
        RawVegan = 16,
        Pesketarijansko = 32,
        CrueltyFree = 64,
        Bio = 128,
        SoyFree = 256,
        NutFree = 512
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
        public string ImageName { get; set; }
        public string Description { get; set; }
        public int ProductClassifiers { get; set; }
        public string Type { get; set; }
        public DateTime AddedTimestamp { get; set; }
        public DateTime LastUpdateTimestamp { get; set; }

        public void Update(Product product)
        {
            product.Name = Name;
            product.Brand = Brand;
            product.Barcode = Barcode;
            product.Description = Description;
            product.ProductClassifiers = ProductClassifiers;
            product.Type = Type;
        }
    }
}
