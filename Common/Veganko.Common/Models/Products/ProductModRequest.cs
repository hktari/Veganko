using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Veganko.Common.Models.Products
{
    public class ProductModRequestEvaluation 
    {
        public string Id { get; set; }
        public string EvaluatorUserId { get; set; }
        public DateTime Timestamp { get; set; }
        public ProductModRequestState State { get; set; }
        public ProductModRequest ProductModRequest { get; set; }
    }

    public class ProductModRequest
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        /// <summary>
        /// The reference to the product being edited with this mod request.
        /// </summary>
        public string ExistingProductId { get; set; }

        public ProductModRequestAction Action { get; set; }
        
        public DateTime Timestamp { get; set; }
        
        [Required]
        public UnapprovedProduct UnapprovedProduct { get; set; }
        
        public string ChangedFields { get; set; }

        public ProductModRequestState State { get; set; }

        public ICollection<ProductModRequestEvaluation> Evaluations { get; set; }

        public void Update(ProductModRequest input)
        {
            ChangedFields = input.ChangedFields;
            UnapprovedProduct?.Update(input.UnapprovedProduct);
        }
    }
}
