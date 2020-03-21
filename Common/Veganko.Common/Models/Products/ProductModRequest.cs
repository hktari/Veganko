using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Common.Models.Products
{
    public class ProductModRequest
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public ProductModRequestAction Action { get; set; }
        public DateTime Timestamp { get; set; }
        public Product Product { get; set; }
        public List<string> Changes { get; set; }
    }
}
