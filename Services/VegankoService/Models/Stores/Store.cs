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
    }

    public class Coordinates
    {
        public string Id { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
