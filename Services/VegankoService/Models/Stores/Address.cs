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

        public override bool Equals(object obj)
        {
            return obj is Address address &&
                   Id == address.Id &&
                   FormattedAddress == address.FormattedAddress &&
                   Street == address.Street &&
                   StreetNumber == address.StreetNumber &&
                   PostalCode == address.PostalCode &&
                   Town == address.Town &&
                   Country == address.Country &&
                   RawJson == address.RawJson &&
                   RetrievedFrom == address.RetrievedFrom;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Id);
            hash.Add(FormattedAddress);
            hash.Add(Street);
            hash.Add(StreetNumber);
            hash.Add(PostalCode);
            hash.Add(Town);
            hash.Add(Country);
            hash.Add(RawJson);
            hash.Add(RetrievedFrom);
            return hash.ToHashCode();
        }
    }
}
