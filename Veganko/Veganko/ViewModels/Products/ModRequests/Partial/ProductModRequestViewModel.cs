using System;
using System.Collections.Generic;
using Veganko.Common.Models.Products;
using Veganko.ViewModels.Products.Partial;

namespace Veganko.ViewModels.Products.ModRequests.Partial
{
    public class ProductModRequestViewModel : BaseViewModel
    {
        public ProductModRequestViewModel(ProductModRequestDTO model)
        {
            Update(model);
        }

        public void Update(ProductModRequestDTO model)
        {
            Id = model.Id;
            UserId = model.UserId;
            ExistingProductId = model.ExistingProductId;

            Action = model.Action;
            Timestamp = model.Timestamp;
            UnapprovedProduct = new ProductViewModel(model.UnapprovedProduct);
            ChangedFields = model.ChangedFields;
            State = model.State;
            Evaluations = new List<ProductModRequestEvaluation>(model.Evaluations);
        }

        public string Id { get; set; }

        public string UserId { get; set; }

        public string ExistingProductId { get; set; }

        public string ChangedFields { get; set; }

        private ProductModRequestAction action;
        public ProductModRequestAction Action
        {
            get => action;
            set => SetProperty(ref action, value);
        }

        private DateTime timestamp;
        public DateTime Timestamp
        {
            get => timestamp;
            set => SetProperty(ref timestamp, value);
        }

        private ProductViewModel unapprovedProduct;
        public ProductViewModel UnapprovedProduct
        {
            get => unapprovedProduct;
            set => SetProperty(ref unapprovedProduct, value);
        }

        private ProductModRequestState state;
        public ProductModRequestState State
        {
            get => state;
            set => SetProperty(ref state, value);
        }

        private ICollection<ProductModRequestEvaluation> evaluations;
        public ICollection<ProductModRequestEvaluation> Evaluations
        {
            get => evaluations;
            set => SetProperty(ref evaluations, value);
        }
    }
}
