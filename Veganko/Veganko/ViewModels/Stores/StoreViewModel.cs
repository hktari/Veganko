using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Models.Stores;
using Veganko.Validations;
using Veganko.ViewModels.Stores.Partial;

namespace Veganko.ViewModels.Stores
{
    public class StoreViewModel : BaseViewModel
    {
        public StoreViewModel(string productId)
            : this()
        {
            ProductId = productId;
        }

        public StoreViewModel(Store store)
            : this()
        {
            Update(store);
        }

        private StoreViewModel()
        {
            Name = new ValidatableObject<string>();
            Price = new ValidatableObject<double>();
            Address = new AddressViewModel(new Address());

            Name.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = "Polje je obvezno" });
            Price.Validations.Add(new IsDoubleInRange(0.01) { ValidationMessage = "Dodaj še ceno" });
        }

        public string Id { get; set; }

        public string ProductId { get; set; }

        public AddressViewModel Address { get; set; }

        public Coordinates? Coordinates { get; set; }

        public ValidatableObject<string> Name { get; }

        public ValidatableObject<double> Price { get; }

        private bool coordinatesFound;
        public bool CoordinatesFound
        {
            get
            {
                return coordinatesFound;
            }
            set
            {
                SetProperty(ref coordinatesFound, value);
            }
        }

        public Coordinates StoreCoordinates { get; private set; }

        public bool Validate()
        {
            bool isValid = true;
            isValid &= Name.Validate();
            isValid &= Address.Validate();
            isValid &= Price.Validate();
            return isValid;
        }

        public void Update(PickStoreViewModel.PickStoreResult storeData)
        {
            Name.Value = storeData.Name;
            Coordinates = storeData.Coordinates;
            Address.Update(storeData.Address);
            CoordinatesFound = storeData.Coordinates != null;
        }

        public void Update(Store store)
        {
            Id = store.Id;
            ProductId = store.ProductId;
            Name.Value = store.Name;
            Price.Value = store.Price;
            Coordinates = store.Coordinates;

            if (store.Address != null)
            {
                Address.Update(store.Address);
            }
        }

        public void MapToModel(Store store)
        {
            store.Id = Id;
            store.ProductId = ProductId;
            store.Name = Name.Value;
            store.Price = Price.Value;
            store.Coordinates = Coordinates;

            if (store.Address == null)
            {
                store.Address = new Address();
            }
            
            Address.MapToModel(store.Address);
        }
    }
}
