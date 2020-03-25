using System;

namespace Veganko.Common.Models.Products
{
    /// <summary>
    /// Duplicate of <see cref="Product"/> to force EF to create a seperate table. I.e. to use the Table-per-Concrete Class (TPC) pattern
    /// See for more info: https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/inheritance?view=aspnetcore-2.1
    /// </summary>
    public class UnapprovedProduct
    {
        public UnapprovedProduct()
        {
        }

        public UnapprovedProduct(Product product)
        {
            MapToUnapprovedProduct(product);
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

        public void Update(UnapprovedProduct product)
        {
            Name = product.Name;
            Brand = product.Brand;
            Barcode = product.Barcode;
            Description = product.Description;
            ProductClassifiers = product.ProductClassifiers;
            Type = product.Type;
        }

        /// <summary>
        /// Maps this <see cref="UnapprovedProduct"/> to the given <see cref="Product"/>.
        /// </summary>
        /// <param name="product">The product to map.</param>
        /// <param name="mapAllFields">Whether all, including read-only fields, should be mapped.</param>
        public void MapToProduct(Product product, bool mapAllFields = false)
        {
            if (mapAllFields)
            {
                product.Id = Id;
                product.ImageName = ImageName;
                product.AddedTimestamp = AddedTimestamp;
                product.LastUpdateTimestamp = LastUpdateTimestamp;
            }

            product.Name = Name;
            product.Brand = Brand;
            product.Barcode = Barcode;
            product.Description = Description;
            product.ProductClassifiers = ProductClassifiers;
            product.Type = Type;
        }

        public void MapToUnapprovedProduct(Product product)
        {
            Id = product.Id;
            ImageName = product.ImageName;
            AddedTimestamp = product.AddedTimestamp;
            LastUpdateTimestamp = product.LastUpdateTimestamp;
            Name = product.Name;
            Brand = product.Brand;
            Barcode = product.Barcode;
            Description = product.Description;
            ProductClassifiers = product.ProductClassifiers;
            Type = product.Type;
        }
    }
}
