using System;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using Veganko.Common.Extensions;
using System.ComponentModel;

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

    public enum ProductType
    {
        [Description("Brez filtra")]
        [EnumMember(Value = "NOT_SET")]
        NOT_SET,

        [Description("Ostalo")]
        [EnumMember(Value = "OTHER")]
        Ostalo,

        [Description("Hrana")]
        [EnumMember(Value = "FOOD")]
        Hrana,

        [Description("Pijača")]
        [EnumMember(Value = "BEVERAGE")]
        Pijaca,

        [Description("Kozmetika")]
        [EnumMember(Value = "COSMETIC")]
        Kozmetika
    }

    public class Product
    {
        public Product(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Brand = product.Brand;
            Barcode = product.Barcode;
            ImageName = product.ImageName;
            Description = product.Description;
            ProductClassifiers = product.ProductClassifiers;
            Type = product.Type;
            AddedTimestamp = product.AddedTimestamp;
            LastUpdateTimestamp = product.LastUpdateTimestamp;
        }
        public Product()
        {
        }

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

        [NotMapped]
        [JsonIgnore]
        public string DetailImage { get; set; }

        [NotMapped]
        [JsonIgnore]
        public string ThumbImage { get; set; }

        // Workaround for the field "Type" being already used as a string inside the database.
        // A better way would be to update the string inside the db to enums (integers).
        [NotMapped]
        [JsonIgnore]
        public ProductType ProdType
        {
            get
            {
                foreach (ProductType val in (ProductType[])Enum.GetValues(typeof(ProductType)))
                {
                    if (val.GetName() == Type)
                    {
                        return val;
                    }
                }

                return ProductType.NOT_SET;
            }
            set => Type = value.GetName();
        }

        public void Update(Product product)
        {
            Name = product.Name;
            Brand = product.Brand;
            Barcode = product.Barcode;
            Description = product.Description;
            ProductClassifiers = product.ProductClassifiers;
            Type = product.Type;
        }
    }
}
