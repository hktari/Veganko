using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Common.Models.Products
{
    /// <summary>
    /// Duplicate of <see cref="Product"/> to force EF to create a seperate table. I.e. to use the Table-per-Concrete Class (TPC) pattern
    /// See for more info: https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/inheritance?view=aspnetcore-2.1
    /// </summary>
    public class UnapprovedProduct
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

        public void Update(UnapprovedProduct product)
        {
            product.Name = Name;
            product.Brand = Brand;
            product.Barcode = Barcode;
            product.Description = Description;
            product.ProductClassifiers = ProductClassifiers;
            product.Type = Type;
        }

        public void MapToProduct(Product product)
        {
            product.Name = Name;
            product.Brand = Brand;
            product.Barcode = Barcode;
            product.ImageName = ImageName;
            product.Description = Description;
            product.ProductClassifiers = ProductClassifiers;
            product.Type = Type;
        }
    }
}
