using Autofac;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Veganko.Models;
using Xamarin.Forms;

namespace Veganko.Services
{
    public class MockProductDataStore : IProductService
    {
        List<Product> items;

        private readonly IAccountService accountService;

        public MockProductDataStore()
        {
            accountService = App.IoC.Resolve<IAccountService>();

            items = new List<Product>();
            var mockItems = new List<Product>
            {
              new Product
                {
                    Id = "0",
                    Name = "Violife Original Flavor", Description = "With coconut oil and vitamin B12",
                    Image = "violife.jpg", Rating = 5,
                    Type = ProductType.Hrana,
                    ProductClassifiers = new ObservableCollection<ProductClassifier>
                    {
                        ProductClassifier.Vegansko,
                    },
                    State = ProductState.Approved
                },
              new Product
                {
                    Id = "1",
                    Name = "Olivella hranilna krema", Description = "Lahka naravna nočna hranilna krema se dobro vpija. Kože ne pušča mastne, temveč jo izjemno hrani in neguje, poživi, ter koži daje mehak in zdrav občutek. Primerna tudi za dnevno nego.",
                    Image = "Olivella_hranilna_krema_r.jpg", Rating = 5,
                    Type = ProductType.Kozmetika,
                    ProductClassifiers = new ObservableCollection<ProductClassifier>
                    {
                        ProductClassifier.CrueltyFree,
                        ProductClassifier.Vegansko,
                        ProductClassifier.GlutenFree
                    },
                    State = ProductState.Approved
                },
              new Product
                {
                    Id = "2",
                    Name = "Violife Mozzarella flavour Grated", Description = "Try making your own pizza and use our vegan mozzarella flavour grated cheese with fresh tomato puree. For the perfect family meal.",
                    Brand = "Violife",
                    Image = "violife_mozarella.png", Rating = 5,
                    Type = ProductType.Hrana,
                    ProductClassifiers = new ObservableCollection<ProductClassifier>
                    {
                        ProductClassifier.Vegansko
                    },
                    State = ProductState.Approved
                },
                new Product
                {
                    Id = "3",
                    Name = "Flow kosmetiikka karitejevo maslo in ognjič", Description = "Organsko karitejevo maslo je primerno za zaščito, nego in vlaženje kože celega telesa. Učinkovito tudi pri negi nog – zmehča trdo in popokano kožo pet.",
                    Image = "Olivella_hranilna_krema_r.jpg",
                    Type = ProductType.Kozmetika,
                    ProductClassifiers = new ObservableCollection<ProductClassifier>
                    {
                        ProductClassifier.Vegansko,
                    },
                    State = ProductState.Approved
                },
                new Product
                {
                    Id = "4",
                    Name = "Čokoladni namaz", Description = "Kdo pa nima rad nutelle... Še posebej, če je vegan.",
                    Image = "evrokrem.jpg",
                    Type = ProductType.Hrana,
                    ProductClassifiers = new ObservableCollection<ProductClassifier>
                    {
                        ProductClassifier.Vegansko,
                        ProductClassifier.GlutenFree
                    },
                    State = ProductState.Approved
                },
                new Product
                {
                    Id = "5",
                    Name = "Knusprige Vollkornwaffeln", Description = "100% Vollkorn und weniger Zucker !",
                    Image = "manner.jpg",
                    Type = ProductType.Hrana,
                    ProductClassifiers = new ObservableCollection<ProductClassifier>
                    {
                        ProductClassifier.Vegansko
                    },
                    State = ProductState.Approved
                },
                new Product
                {
                    Id = "6",
                    Name = "Sensitiv After Shave Balsam", Description = "MEN",
                    Image = "alverde_after_shave.jpg",
                    Type = ProductType.Kozmetika,
                    ProductClassifiers = new ObservableCollection<ProductClassifier>
                    {
                        ProductClassifier.CrueltyFree,
                        ProductClassifier.Vegansko
                    },
                    State = ProductState.Approved
                },
                new Product
                {
                    Id = "7",
                    Name = "Valsoia la crema", Description = "Kremni namaz z lešniki, kakavom in sojo",
                    Brand = "VALSOIA",
                    Image = "evrokrem.jpg",
                    Type = ProductType.Hrana,
                    ProductClassifiers = new ObservableCollection<ProductClassifier>
                    {
                        ProductClassifier.Vegansko,
                        ProductClassifier.GlutenFree
                    },
                    State = ProductState.Approved
                },
                new Product
                {
                    Id = "8",
                    Name = "Gourmet Arašidov Namaz s koščki",
                    Brand = "GOURMET",
                    Image = "arasidovo_maslo.jpg",
                    Type = ProductType.Hrana,
                    ProductClassifiers = new ObservableCollection<ProductClassifier>
                    {
                        ProductClassifier.Vegansko,
                        ProductClassifier.GlutenFree
                    },
                    State = ProductState.Approved
                },
                new Product
                {
                    Id = "9",
                    Name = "BIO Pomarančni sok",
                    Brand = "DM",
                    Image = "dmbio_orangensaft.jpg",
                    Type = ProductType.Pijaca,
                    ProductClassifiers = new ObservableCollection<ProductClassifier>
                    {
                        ProductClassifier.Vegansko,
                        ProductClassifier.GlutenFree,
                        ProductClassifier.RawVegan,
                        ProductClassifier.Pesketarijansko
                    },
                    State = ProductState.Approved
                },
                new Product
                {
                    Id = "10",
                    Name = "Adez Almond Drink",
                    Description = " With its unique proposition of blended plant ingredients AdeZ is bringing great taste to plant-based beverages for the first time and is a nutritious and tasty option for the morning and throughout the day. Enjoy as a drink, with cereals, in a smoothie, in tea or coffee, even for cooking. AdeZ, nourish your potential.",
                    Brand = "Adez",
                    Image = "adez_almond.jpg",
                    Type = ProductType.Pijaca,
                    ProductClassifiers = new ObservableCollection<ProductClassifier>
                    {
                        ProductClassifier.Vegeterijansko,
                    },
                    State = ProductState.Approved
                }
            };

            foreach (var item in mockItems)
            {
                items.Add(item);
            }
        }

        public async Task<bool> AddAsync(Product item)
        {
            // TODO: uncomment when done testing
            //if (accountService.User.CanApproveProducts())
            //{
            //    item.State = ProductState.Approved;
            //}

            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateAsync(Product item)
        {
            var _item = items.Where((Product arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(_item);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync(Product item)
        {
            var _item = items.Where((Product arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(_item);

            return await Task.FromResult(true);
        }

        public async Task<Product> GetAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public Task<IEnumerable<Product>> AllAsync(bool forceRefresh, bool includeUnapproved)
        {
            IEnumerable<Product> result = includeUnapproved ? items : items.Where(p => p.State == ProductState.Approved);
            return Task.FromResult(result);
        }


        public Task<IEnumerable<Product>> GetUnapprovedAsync(bool forceRefresh)
        {
            IEnumerable<Product> result = items.Where(p => p.State == ProductState.PendingApproval);
            return Task.FromResult(result);
        }
    }
}

