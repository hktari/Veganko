using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Veganko.Models;

[assembly: Xamarin.Forms.Dependency(typeof(Veganko.Services.MockProductDataStore))]
namespace Veganko.Services
{
    public class MockProductDataStore : IDataStore<Product>
    {
        List<Product> items;

        public MockProductDataStore()
        {
            items = new List<Product>();
            var mockItems = new List<Product>
            {
              new Product
                {
                    Id = 0,
                    Name = "Vegan Cheese", Description = "100% VEGAN",
                    Image = "raspeberry_meringue.jpg", Rating = 5,
                    Type = ProductType.Hrana,
                    ProductClassifiers = new ObservableCollection<ProductClassifier>
                    {
                        ProductClassifier.Vegansko,
                        ProductClassifier.GlutenFree
                    }
                },
                new Product
                {
                    Id = 1,
                    Name = "Lepotna krema", Description = "Za fajn namazane roke",
                    Image = "raspeberry_meringue.jpg",
                    Type = ProductType.Kozmetika,
                    ProductClassifiers = new ObservableCollection<ProductClassifier>
                    {
                        ProductClassifier.Vegansko,
                        ProductClassifier.GlutenFree,
                        ProductClassifier.CrueltyFree
                    }
                },
                new Product
                {
                    Id = 2,
                    Name = "Čokoladni namaz", Description = "Kdo pa nima rad nutelle... Še posebej, če je vegan.",
                    Image = "raspeberry_meringue.jpg",
                    Type = ProductType.Hrana,
                    ProductClassifiers = new ObservableCollection<ProductClassifier>
                    {
                        ProductClassifier.Vegansko,
                        ProductClassifier.GlutenFree
                    }
                }
            };

            foreach (var item in mockItems)
            {
                items.Add(item);
            }
        }

        public async Task<bool> AddItemAsync(Product item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Product item)
        {
            var _item = items.Where((Product arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(_item);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(Product item)
        {
            var _item = items.Where((Product arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(_item);

            return await Task.FromResult(true);
        }

        public async Task<Product> GetItemAsync(int id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Product>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}