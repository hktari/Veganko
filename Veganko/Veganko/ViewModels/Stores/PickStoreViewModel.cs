using Autofac;
using GooglePlacesApi;
using GooglePlacesApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Veganko.Extensions;
using Veganko.Services.Logging;
using Xamarin.Forms;
using Veganko.Models.Stores;

namespace Veganko.ViewModels.Stores
{
    public class PickStoreViewModel : BaseViewModel
    {
        public const string StorePickedMsg = "StorePickedMsg";

        private string searchText;
        private GooglePlacesApiService _api;
        private ILogger logger;

        public PickStoreViewModel()
        {
            logger = App.IoC.Resolve<ILogger>();

            string key = null;

            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(PickStoreViewModel)).Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(App.AssemblyNamespacePrefix + "google_places_api_key.txt"))
            using (StreamReader reader = new StreamReader(stream))
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(reader.ReadToEnd())))
            using (CryptoStream cryptoStream = new CryptoStream(ms, new FromBase64Transform(), CryptoStreamMode.Read))
            using (StreamReader writer = new StreamReader(cryptoStream))
            {
                key = writer.ReadToEnd();
            }

            var settings = GoogleApiSettings.Builder.WithApiKey(key)
                .AddCountry("svn")
              .AddCountry("aut")
              .WithType(PlaceTypes.Establishment)
              .Build();
            _api = new GooglePlacesApiService(settings);
        }

        private List<Prediction> searchResults;
        public List<Prediction> SearchResults
        {
            get
            {
                return searchResults;
            }
            set
            {
                SetProperty(ref searchResults, value);
            }
        }

        private Prediction selectedStore;
        public Prediction SelectedStore
        {
            get
            {
                return selectedStore;
            }
            set
            {
                SetProperty(ref selectedStore, value);
            }
        }

        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                SetProperty(ref searchText, value);
            }
        }

        public Command StartSearchCommand => new Command(
            async () => 
            {
                // TODO: if less than min show error

                var result = await _api.GetPredictionsAsync(SearchText);

                if (result != null && result.Status.Equals("OK"))
                {
                    SearchResults = result.Items;
                }
                else 
                {
                    SearchResults = new List<Prediction>();
                }
            });

        public Command<ItemTappedEventArgs> LocationPickedCommand => new Command<ItemTappedEventArgs>(
            async (_) =>
            {
                try
                {
                    IsBusy = true;
                    Details placeDetails = await _api.GetDetailsAsync(selectedStore.PlaceId, _api.GetSessionToken(), DetailLevel.Basic);

                    List<AddressComponent> addrComp = placeDetails.Place.AddressComponents;
                    Address address = new Address
                    {
                        FormattedAddress = placeDetails.Place.FormattedAddress,
                        RawJson = JsonConvert.SerializeObject(placeDetails.Place.AddressComponents),
                        RetrievedFrom = "google-places",
                        Street = GetNameForAddressCompType("route", addrComp),
                        StreetNumber = GetNameForAddressCompType("street_number", addrComp),
                        Country = GetNameForAddressCompType("country", addrComp),
                        PostalCode = GetNameForAddressCompType("postal_code", addrComp),
                        Town = GetNameForAddressCompType("postal_town", addrComp)
                        ?? GetNameForAddressCompType("locality", addrComp)
                    };

                    PickStoreResult store = new PickStoreResult
                    {
                        Name = placeDetails.Place.Name,
                        Address = address,
                    };

                    if (placeDetails.Place?.Geometry?.Location != null)
                    {
                        store.Coordinates = new Coordinates
                        {
                            Latitude = placeDetails.Place.Geometry.Location.Latitude,
                            Longitude = placeDetails.Place.Geometry.Location.Longitude,
                        };
                    }

                    MessagingCenter.Send(
                        this,
                        StorePickedMsg,
                        store);
                }
                catch (Exception ex)
                {
                    logger.LogException(ex);
                    await App.CurrentPage.Err("Neznana napaka");
                }
                finally
                {
                    await App.Navigation.PopModalAsync();
                    IsBusy = false;
                }
            });

        private static string GetNameForAddressCompType(string type, List<AddressComponent> addrComp)
        {
            return addrComp?.FirstOrDefault(adr => adr.Types?.Contains(type) ?? false)?.LongName;
        }

        public struct PickStoreResult
        {
            public string Name { get; set; }
            
            public Address Address { get; set; }

            public Coordinates? Coordinates { get; set; }
        }
    }
}
