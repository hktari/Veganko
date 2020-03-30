using System;
using System.Collections.Generic;
using Veganko.Common.Models.Products;
using Veganko.ViewModels.Products.Partial;

namespace Veganko.ViewModels.Products.ModRequests.Partial
{
    public class ProductModRequestViewModel : ProductViewModel
    {
        public ProductModRequestViewModel(ProductModRequestDTO model)
        {
            Update(model);
        }

        public ProductModRequestDTO Model { get; }

        private DateTime timestamp;
        public DateTime Timestamp
        {
            get => timestamp;
            set => SetProperty(ref timestamp, value);
        }

        private ProductModRequestState state;
        public ProductModRequestState State
        {
            get => state;
            set => SetProperty(ref state, value);
        }

        private string evaluatorsText;
        public string EvaluatorsText
        {
            get => evaluatorsText;
            set => SetProperty(ref evaluatorsText, value);
        }

        private List<string> changedFields;
        public List<string> ChangedFields
        {
            get => changedFields;
            set => SetProperty(ref changedFields, value);
        }

        public void Update(ProductModRequestDTO model)
        {
            // Update the product properties
            base.Update(model.UnapprovedProduct);

            ProductStateIndicator indState = CurIndicatorState;
            switch (model.State)
            {
                case ProductModRequestState.Pending:
                    indState = ProductStateIndicator.Pending;
                    break;
                case ProductModRequestState.Approved:
                    indState = ProductStateIndicator.Approved;
                    break;
                case ProductModRequestState.Rejected:
                    indState = ProductStateIndicator.Rejected;
                    break;
                case ProductModRequestState.Missing:
                    indState = ProductStateIndicator.Missing;
                    break;
                default:
                    break;
            }

            SetStateIndicatorImage(indState);
            Timestamp = model.Timestamp;
            State = model.State;
            // TODO
            //EvaluationsText = model.Evaluations.Aggregate("", (str, pmr) => str += pmr.)
        }

        /// <summary>
        /// Returns the updated instance of <see cref="ProductModRequestDTO"/> that was received when constructing this object.
        /// </summary>
        /// <returns></returns>
        public ProductModRequestDTO GetModel()
        {
            // TODO:
            //ProductModReq.ChangedFields = ChangedFields
            Model.UnapprovedProduct = MapToModel();
            return Model;
        }
    }
}
