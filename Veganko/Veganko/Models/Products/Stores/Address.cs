using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Models.Products.Stores
{
    public class Address
    {
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
