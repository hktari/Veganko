using Autofac;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Veganko.Common.Models.Products;
using Veganko.Models;
using Xamarin.Forms;

namespace Veganko.Services
{
    public class MockProductDataStore : IProductService
    {
        public List<Product> Items { get; set; }

        public MockProductDataStore()
        {
            Items = new List<Product>
            {
            //  new Product
            //    {
            //        Id = "0",
            //        Name = "Violife Original Flavor", Description = "With coconut oil and vitamin B12",
            //        Brand = "Violife",
            //        ThumbImage =  "violife.jpg",
            //        DetailImage = "violife.jpg",
            //        Type = ProductType.Hrana,
            //        ProductClassifiers = new List<ProductClassifier>
            //        {
            //            ProductClassifier.Vegansko,
            //        },
            //        State = ProductState.Approved,
            //        AddedTimestamp = DateTime.Now - TimeSpan.FromHours(2),
            //    },
            //  new Product
            //    {
            //        Id = "1",
            //        Brand = "A rather long brand name which does cosmetics and stuff.",
            //        Name = "Olivella hranilna krema", Description = "Lahka naravna nočna hranilna krema se dobro vpija. Kože ne pušča mastne, temveč jo izjemno hrani in neguje, poživi, ter koži daje mehak in zdrav občutek. Primerna tudi za dnevno nego.",
            //        ThumbImage =  "Olivella_hranilna_krema_r.jpg",
            //        DetailImage = "Olivella_hranilna_krema_r.jpg",
            //        Type = ProductType.Kozmetika,
            //        ProductClassifiers = new List<ProductClassifier>
            //        {
            //            ProductClassifier.CrueltyFree,
            //            ProductClassifier.Vegansko,
            //            ProductClassifier.GlutenFree
            //        },
            //        State = ProductState.Approved,
            //        AddedTimestamp = DateTime.Now - TimeSpan.FromDays(2.5),
            //    },
            //  new Product
            //    {
            //        Id = "2",
            //        Name = "Violife Mozzarella flavour Grated", Description = "Try making your own pizza and use our vegan mozzarella flavour grated cheese with fresh tomato puree. For the perfect family meal.",
            //        Brand = "Violife",
            //        ThumbImage =  "violife_mozarella.png",
            //        DetailImage = "violife_mozarella.png",
            //        Type = ProductType.Hrana,
            //        ProductClassifiers = new List<ProductClassifier>
            //        {
            //            ProductClassifier.Vegansko
            //        },
            //        State = ProductState.Approved,
            //        AddedTimestamp = DateTime.Now - TimeSpan.FromHours(1),
            //    },
            //new Product
            //{
            //    Id = "3",
            //    Name = "Flow kosmetiikka karitejevo maslo in ognjič", Description = "Organsko karitejevo maslo je primerno za zaščito, nego in vlaženje kože celega telesa. Učinkovito tudi pri negi nog – zmehča trdo in popokano kožo pet.",
            //    ThumbImage =  "Olivella_hranilna_krema_r.jpg",
            //    DetailImage = "Olivella_hranilna_krema_r.jpg",
            //    Type = ProductType.Kozmetika,
            //    ProductClassifiers = new List<ProductClassifier>
            //    {
            //        ProductClassifier.Vegansko,
            //    },
            //    State = ProductState.Approved,
            //    AddedTimestamp = DateTime.Now - TimeSpan.FromDays(1),
            //},
            //new Product
            //{
            //    Id = "4",
            //    Name = "Čokoladni namaz", Description = "Kdo pa nima rad nutelle... Še posebej, če je vegan.",
            //    ThumbImage =  "evrokrem.jpg",
            //    DetailImage = "evrokrem.jpg",
            //    Type = ProductType.Hrana,
            //    ProductClassifiers = new List<ProductClassifier>
            //    {
            //        ProductClassifier.Vegansko,
            //        ProductClassifier.GlutenFree
            //    },
            //    State = ProductState.Approved,
            //    AddedTimestamp = DateTime.Now - TimeSpan.FromHours(0.25),
            //},
            //new Product
            //{
            //    Id = "5",
            //    Name = "Knusprige Vollkornwaffeln", Description = "100% Vollkorn und weniger Zucker !",
            //    ThumbImage =  "manner.jpg",
            //    DetailImage = "manner.jpg",
            //    Type = ProductType.Hrana,
            //    ProductClassifiers = new List<ProductClassifier>
            //    {
            //        ProductClassifier.Vegansko
            //    },
            //    State = ProductState.Approved
            //},
            //new Product
            //{
            //    Id = "6",
            //    Name = "Sensitiv After Shave Balsam", Description = "MEN",
            //    ThumbImage =  "alverde_after_shave.jpg",
            //    DetailImage = "alverde_after_shave.jpg",
            //    Type = ProductType.Kozmetika,
            //    ProductClassifiers = new List<ProductClassifier>
            //    {
            //        ProductClassifier.CrueltyFree,
            //        ProductClassifier.Vegansko
            //    },
            //    State = ProductState.Approved
            //},
            //new Product
            //{
            //    Id = "7",
            //    Name = "Valsoia la crema", Description = "Kremni namaz z lešniki, kakavom in sojo",
            //    Brand = "VALSOIA",
            //    ThumbImage =  "evrokrem.jpg",
            //    DetailImage = "evrokrem.jpg",
            //    Type = ProductType.Hrana,
            //    ProductClassifiers = new List<ProductClassifier>
            //    {
            //        ProductClassifier.Vegansko,
            //        ProductClassifier.GlutenFree
            //    },
            //    State = ProductState.Approved
            //},
            //new Product
            //{
            //    Id = "8",
            //    Name = "Gourmet Arašidov Namaz s koščki",
            //    Brand = "GOURMET",
            //    ThumbImage =  "arasidovo_maslo.jpg",
            //    DetailImage = "arasidovo_maslo.jpg",
            //    Type = ProductType.Hrana,
            //    ProductClassifiers = new List<ProductClassifier>
            //    {
            //        ProductClassifier.Vegansko,
            //        ProductClassifier.GlutenFree
            //    },
            //    State = ProductState.Approved,
            //    AddedTimestamp = DateTime.Now- TimeSpan.FromHours(0.5),
            //},
            //new Product
            //{
            //    Id = "9",
            //    Name = "BIO Pomarančni sok",
            //    Brand = "DM",
            //    ThumbImage =  "dmbio_orangensaft.jpg",
            //    DetailImage = "dmbio_orangensaft.jpg",
            //    Type = ProductType.Pijaca,
            //    ProductClassifiers = new List<ProductClassifier>
            //    {
            //        ProductClassifier.Vegansko,
            //        ProductClassifier.GlutenFree,
            //        ProductClassifier.RawVegan,
            //        ProductClassifier.Pesketarijansko
            //    },
            //    State = ProductState.Approved
            //},
            //new Product
            //{
            //    Id = "10",
            //    Name = "Adez Almond Drink",
            //    Description = " With its unique proposition of blended plant ingredients AdeZ is bringing great taste to plant-based beverages for the first time and is a nutritious and tasty option for the morning and throughout the day. Enjoy as a drink, with cereals, in a smoothie, in tea or coffee, even for cooking. AdeZ, nourish your potential.",
            //    Brand = "Adez",
            //    ThumbImage =  "adez_almond.jpg",
            //    DetailImage = "adez_almond.jpg",
            //    Type = ProductType.Pijaca,
            //    ProductClassifiers = new List<ProductClassifier>
            //    {
            //        ProductClassifier.Vegeterijansko,
            //    },
            //    State = ProductState.Approved
            //},
            };

            Items = Items.Concat(
                Enumerable
                .Range(Items.Count + 10, 500)
                .Select(idx => CreateGenericProduct(idx))).ToList();
        }

        private Product CreateGenericProduct(int idx)
        {
            return new Product
            {
                Id = idx.ToString(),
                LastUpdateTimestamp = DateTime.Now,
                ThumbImage = "adez_almond.jpg",
                ProductClassifiers = 0,
                ProdType = ProductType.Pijaca,
                Name = "Generic product",
                Description = "generic description",
                Brand = "Generic brand"
            };
        }

        public Task<IEnumerable<Product>> GetUnapprovedAsync(bool forceRefresh)
        {
            throw new NotImplementedException();
        }

        Task<Product> IProductService.AddAsync(Product item)
        {
            Items.Add(item);

            return Task.FromResult(item);
        }

        Task<Product> IProductService.UpdateAsync(Product item)
        {
            var _item = Items.Where((Product arg) => arg.Id == item.Id).FirstOrDefault();
            Items.Remove(_item);
            Items.Add(item);

            return Task.FromResult(item);
        }

        Task IProductService.DeleteAsync(Product item)
        {
            var _item = Items.Where((Product arg) => arg.Id == item.Id).FirstOrDefault();
            Items.Remove(_item);

            return Task.CompletedTask;
        }

        Task<PagedList<Product>> IProductService.AllAsync(int page = 1, int pageSize = 10, bool forceRefresh = false, bool includeUnapproved = false)
        {
            IEnumerable<Product> result = Items;
            return Task.FromResult(new PagedList<Product> { Items = result });
        }

        Task<Product> IProductService.GetAsync(string id)
        {
            return Task.FromResult(Items.First(p => p.Id == id));
        }

        public Task<Product> UpdateImagesAsync(Product product, byte[] detailImageData, byte[] thumbImageData)
        {
            // TODO: figure out something
            //product.DetailImage = ImageSource.FromStream(() => new MemoryStream(detailImageData));
            //product.ThumbImage = ImageSource.FromStream(() => new MemoryStream(thumbImageData));
            return Task.FromResult(product);
        }
    }
}

