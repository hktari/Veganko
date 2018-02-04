using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Veganko.Models;

[assembly: Xamarin.Forms.Dependency(typeof(Veganko.Services.MockDataStore))]
namespace Veganko.Services
{
    public class MockDataStore : IDataStore<Product>
    {
        List<Product> items;

        public MockDataStore()
        {
            items = new List<Product>();
            var mockItems = new List<Product>
            {
              new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Vegan Cheese", Description = "100% VEGAN",
                    Image = "raspeberry_meringue.jpg", Rating = 5,
                    Type = ProductType.Hrana,
                    ProductClassifiers = new ObservableCollection<ProductClassifier>
                    {
                        ProductClassifier.Vegansko,
                        ProductClassifier.GlutenFree
                    },
                    Comments = new ObservableCollection<Comment>
                    {
                        new Comment
                        {
                            Id = Guid.NewGuid().ToString(),
                            Username = "BigDick112",
                            Rating = 4,
                            DatePosted = DateTime.Now,
                            Text = "Res ful dobro... Močno priporočam."
                        },
                        new Comment
                        {
                            Id = Guid.NewGuid().ToString(),
                            Username = "Magda_likesbigdick113",
                            Rating = 3,
                            DatePosted = DateTime.Now,
                            Text = "Sreča je kot metulj."
                        },
                        new Comment
                        {
                            Id = Guid.NewGuid().ToString(),
                            Username = "Janez_iz_portoroža",
                            Rating = 2,
                            DatePosted = DateTime.Now,
                            Text = "Nima točno takšnega okusa kot nutella :/"
                        },
                        new Comment
                        {
                            Id = Guid.NewGuid().ToString(),
                            Username = "Ed Sheeran",
                            Rating = 5,
                            DatePosted = DateTime.Now,
                            Text = "Real great stuff ! I should write a song about it..."
                        },
                        new Comment
                        {
                            Id = Guid.NewGuid().ToString(),
                            Username = "zalathecat",
                            Rating = 5,
                            DatePosted = DateTime.Now,
                            Text = "Čokolada je life. In seveda mačke..."
                        }
                    }
                },
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
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
                    Id = Guid.NewGuid().ToString(),
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

        public async Task<Product> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Product>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}