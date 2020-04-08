using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Veganko.Common.Models.Products
{
    // TODO: move to VegankoService project
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

        public UnapprovedProduct UnapprovedProduct { get; set; }

        public string ChangedFields { get; set; }

        public ProductModRequestState State { get; set; }

        public ICollection<ProductModRequestEvaluation> Evaluations { get; set; } = new List<ProductModRequestEvaluation>();

        public void Update(ProductModRequest input)
        {
            ChangedFields = input.ChangedFields;
            UnapprovedProduct?.Update(input.UnapprovedProduct);
        }
    }
}
