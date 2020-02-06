using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VegankoService.Models.Stores
{
    public class Address
    {
        public string Id { get; set; }

        [Required]
        public string FormattedAddress { get; set; }

        public string Street { get; set; }

        public string StreetNumber { get; set; }

        public string PostalCode { get; set; }

        public string Town { get; set; }

        public string Country { get; set; }

        public string RawJson { get; set; }

        public string RetrievedFrom { get; set; }
    }
}
