using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Common.Models.Products;
using Veganko.ViewModels.Products.Partial;

namespace Veganko.ViewModels.Products.ModRequests.Partial
{
    public class ProductModRequestViewModel : BaseViewModel
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string ExistingProductId { get; set; }

        public ProductModRequestAction Action { get; set; }

        public DateTime Timestamp { get; set; }

        public ProductViewModel UnapprovedProduct { get; set; }

        public string ChangedFields { get; set; }

        public ProductModRequestState State { get; set; }

        public ICollection<ProductModRequestEvaluation> Evaluations { get; set; }
    }
}
