using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Veganko.Common.Models.Products
{
    // TODO: move to VegankoService project
    public class ProductModRequest
    {
        public ProductModRequest()
        {
            UnapprovedProduct = new UnapprovedProduct();
        }

        public string Id { get; set; }

        public string UserId { get; set; }

        /// <summary>
        /// The reference to the product that has been created with this mod request (action NEW).
        /// </summary>
        public string NewlyCreatedProductId { get; set; }

        /// <summary>
        /// The reference to the product being edited with this mod request (action EDIT).
        /// </summary>
        public string ExistingProductId { get; set; }

        public ProductModRequestAction Action { get; set; }

        public DateTime Timestamp { get; set; }

        // Attribute tells EF to on delete cascade delete the product as well.
        [Required]
        public UnapprovedProduct UnapprovedProduct { get; set; }

        public string ChangedFields { get; set; }

        public ProductModRequestState State { get; set; }

        public ICollection<ProductModRequestEvaluation> Evaluations { get; set; } = new List<ProductModRequestEvaluation>();

        public void Update(ProductModRequest input)
        {
            ExistingProductId = input.ExistingProductId;
            ChangedFields = input.ChangedFields;
            UnapprovedProduct.Update(input.UnapprovedProduct);
        }
    }
}
