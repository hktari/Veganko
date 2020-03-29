using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VegankoService.Models.Stores
{
    public class Store
    {
        public string Id { get; set; }

        [Required]
        public string ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public Address Address { get; set; }

        [Required]
        public double Price { get; set; }

        public Coordinates Coordinates { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Store store &&
                   Id == store.Id &&
                   ProductId == store.ProductId &&
                   Name == store.Name &&
                   EqualityComparer<Address>.Default.Equals(Address, store.Address) &&
                   Price == store.Price &&
                   EqualityComparer<Coordinates>.Default.Equals(Coordinates, store.Coordinates);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, ProductId, Name, Address, Price, Coordinates);
        }
    }

    public class Coordinates
    {
        public string Id { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Coordinates coordinates &&
                   Id == coordinates.Id &&
                   Latitude == coordinates.Latitude &&
                   Longitude == coordinates.Longitude;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Latitude, Longitude);
        }
    }
}
