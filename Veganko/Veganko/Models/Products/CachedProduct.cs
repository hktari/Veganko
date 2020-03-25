using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Common.Models.Products;

namespace Veganko.Models.Products
{
    public class CachedProduct
    {
        public CachedProduct()
        {
        }

        public CachedProduct(Product product)
        {
            ProductId = product.Id;
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        [Unique]
        public string ProductId { get; set; }
        
        public DateTime LastSeenTimestamp { get; set; }

        public bool HasBeenSeen { get; set; }
    }
}
