using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Models.Products.Stores
{
    public class Store
    {
        public string Id { get; set; }
        
        public string ProductId { get; set; }

        public string Name { get; set; }
        
        public Address Address { get; set; }
        
        public double Price { get; set; }
        
        public Coordinates? Coordinates { get; set; }
    }

    public struct Coordinates
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
