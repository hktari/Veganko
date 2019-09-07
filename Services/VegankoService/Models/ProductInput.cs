using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Models.Converters;

namespace VegankoService.Models
{
    public class ProductInput
    {
        [Required]
        public string Name { get; set; }

        public string Brand { get; set; }

        public string Barcode { get; set; }

        [Required]
        public string ImageBase64Encoded { get; set; }

        public string Description { get; set; }

        [Required]
        public int ProductClassifiers { get; set; }

        [Required]
        public string Type { get; set; }

        public void MapToProduct(Product product)
        {
            product.Name = Name;
            product.Brand = Brand;
            product.Barcode = Barcode;
            product.ImageBase64Encoded = ImageBase64Encoded;
            product.Description = Description;
            product.ProductClassifiers = ProductClassifiers;
            product.Type = Type;
        }
    }
}
