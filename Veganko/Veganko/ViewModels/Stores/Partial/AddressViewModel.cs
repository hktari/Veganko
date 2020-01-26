using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Models.Stores;
using Veganko.Validations;

namespace Veganko.ViewModels.Stores.Partial
{
    public class AddressViewModel : BaseViewModel
    {
        public AddressViewModel(Address address)
        {
            FormattedAddress = new ValidatableObject<string>();
            Update(address);
        }

        public ValidatableObject<string> FormattedAddress { get; }

        public string Street { get; set; }

        public string StreetNumber { get; set; }

        public string PostalCode { get; set; }

        public string Town { get; set; }

        public string Country { get; set; }

        public string RawJson { get; set; }

        public string RetrievedFrom { get; set; }

        public void MapToModel(Address address)
        {
            address.FormattedAddress = FormattedAddress.Value;
            address.Street = Street;
            address.StreetNumber = StreetNumber;
            address.PostalCode = PostalCode;
            address.Town = Town;
            address.Country = Country;
            address.RawJson = RawJson;
            address.RetrievedFrom = RetrievedFrom;
        }

        public void Update(Address address)
        {
            FormattedAddress.Value = address.FormattedAddress;
            Street = address.Street;
            StreetNumber = address.StreetNumber;
            PostalCode = address.PostalCode;
            Town = address.Town;
            Country = address.Country;
            RawJson = address.RawJson;
            RetrievedFrom = address.RetrievedFrom;
        }

        internal bool Validate()
        {
            return FormattedAddress.Validate();
        }
    }
}
