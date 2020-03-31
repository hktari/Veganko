using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Veganko.Common.Models.Products
{
    /// <summary>
    /// Used between client and controller. Database uses <see cref="ProductModRequest"/>.
    /// </summary>
    public class ProductModRequestDTO : ProductModRequest
    {
        public ProductModRequestDTO()
        {
        }

        public ProductModRequestDTO(ProductModRequest model)
        {
            MapToDto(model);
        }

        [Required]
        new public Product UnapprovedProduct { get; set; }

        [JsonIgnore]
        public List<string> ChangedFieldsAsList 
        {
            get
            {
                List<string> changes = new List<string>();
                if (ChangedFields != null)
                {
                    changes = ChangedFields.Split(';').ToList();
                }
                return changes;
            }
            set
            {
                ChangedFields = string.Join(";", value);
            }
        }

        public void MapToDto(ProductModRequest model)
        {
            Id = model.Id;

            UserId = model.UserId;

            ExistingProductId = model.ExistingProductId;

            Action = model.Action;

            Timestamp = model.Timestamp;

            var product = new Product();
            model.UnapprovedProduct.MapToProduct(product, mapAllFields: true);
            UnapprovedProduct = product;

            ChangedFields = model.ChangedFields;

            State = model.State;

            Evaluations = model.Evaluations;
        }

        public ProductModRequest MapToModel()
        {
            ProductModRequest model = new ProductModRequest();

            model.Id = Id;
            model.UserId = UserId;
            model.ExistingProductId = ExistingProductId;
            model.Action = Action;
            model.Timestamp = Timestamp;
            model.UnapprovedProduct = new UnapprovedProduct(UnapprovedProduct);
            model.ChangedFields = ChangedFields;
            model.State = State;
            model.Evaluations = Evaluations;

            return model;
        }
    }
}
