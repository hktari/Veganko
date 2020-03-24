using Newtonsoft.Json;
using System;

namespace Veganko.Common.Models.Products
{
    public class ProductModRequestEvaluation 
    {
        public string Id { get; set; }
        public string EvaluatorUserId { get; set; }
        public DateTime Timestamp { get; set; }
        public ProductModRequestState State { get; set; }

        /// Prevent circular dependency when serializing <see cref="Products.ProductModRequest"/>
        [JsonIgnore]
        public ProductModRequest ProductModRequest { get; set; }
    }
}
