using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VegankoService.Models.Products
{
    public class ProductQuery
    {
        public string Barcode { get; set; }

        public string Text { get; set; }

        public string Type { get; set; }

        public int? Classifiers { get; set; }
    }
}
