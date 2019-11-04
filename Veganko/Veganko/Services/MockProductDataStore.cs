using Autofac;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Veganko.Models;
using Xamarin.Forms;
using System.Linq;
using System.Text;

namespace Veganko.Services
{
    public class MockProductDataStore : IProductService
    {
        List<Product> items;

        public MockProductDataStore()
        {
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
                    Id = "11",
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
                    Id = "12",
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
                    Id = "13",
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
                    Id = "14",
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
                    Id = "15",
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
                    Id = "16",
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
                    Id = "17",
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
                    Id = "18",
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
                    Id = "19",
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
                    Id = "11",
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
                    Id = "12",
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
                    Id = "13",
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
                    Id = "14",
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
                    Id = "15",
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
                    Id = "16",
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
                    Id = "17",
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
                    Id = "18",
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
                    Id = "19",
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
                    Id = "11",
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
                    Id = "12",
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
                    Id = "13",
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
                    Id = "14",
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
                    Id = "15",
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
                    Id = "16",
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
                    Id = "17",
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
                    Id = "18",
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
                    Id = "19",
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
                    Id = "20",
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

            for (int i = 0; i < mockItems.Count; i++)
            {
                mockItems[i].ImageBase64Encoded = i % 2 == 0 ? Convert.FromBase64String(testImg) : Convert.FromBase64String(testImg2);
                items.Add(mockItems[i]);
            }
        }

        const string testImg = @"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";
        const string testImg2 = @"iVBORw0KGgoAAAANSUhEUgAAANIAAAAzCAYAAADigVZlAAAQN0lEQVR4nO2dCXQTxxnHl0LT5jVteHlN+5q+JCKBJITLmHIfKzBHHCCYBAiEw+I2GIMhDQ0kqQolIRc1SV5e+prmqX3JawgQDL64bK8x2Ajb2Bg7NuBjjSXftmRZhyXZ1nZG1eL1eGa1kg2iyua9X2TvzvHNN/Ofb2Z2ZSiO4ygZGZm+EXADZGSCgYAbICMTDATcABmZYCDgBsjIBAMBN0BGJhgIuAEyMsGA1wQdHZ1UV1cX5XK5qM7OzgcMRuNTrSbTEraq6strhdfzruTk5Wpz8q5c1l7Jyb6szc3K1l7RggtFxcWX2dvVB02mtmVOp3NIV2fnQFie2WyB5QS84TIy/YnXBFBI8BMM/pDqat0XzIVM08lTSVxyytn6jAuZV4FuzmtzclJz8/LT8vML0nJzr54HYkpLS88oTkxMMZ48mchlXrxUX1ffcBCUM8xms8lCkgk6pCT6aZvZvCrzYpbu2PfxHAg8l+obGmOt1vaJQBAPkvI5nM5fWyyWWTU1tfuA+IqOHDvGgehVCK4pA91oGZn+xluCAc0thtj4hCT72XOp9S0thi2FBQWPvb13z9RN61QH5s8NYxbMDct7KXyudt7MGeeWLFrwn8iVKz7auDZy3Z7dbzz91p43B8ZsjYLlDKmprd3/ffwpLjWNqbW32xcFuuEyMv2J2M1BJpMpKiExxZKZeamira1tvvqdt8OWL1l8asq4kNbRzz7NTRo7uuMPo4Y7Rz/zFBc64lluzHNDuZFDFe5PICx25/aY2B3bogf/dd9fKCA+CuytohOSkjuyLmtLXRwXGujGy8j0F8Qbdrt9bDpzQQ8jSHl5+dLt0VsOThgzwj7i6Se5kOHDuIljR9mXRrykjZj/wlVeSONHP8+FhykrJoeOsY8aNoQLAYJa9erShIPvvRsKhQTK/YleX3Pw5KlErpKt+iLQjZeR6S9IN35VXl75r3gw4HU6/Z6ojes/gMKAUQiKBQKiUvvLC1/MXL18WcKsaZOrJ4WObly7euUJsOQ7FjZ9Sh2IVC4oLhihZk6d1LB5/dpt+9R/hnuq4Xl5VwvT0jLKXS7XOHgaCAm0I2Rk+gL2os1mewXsiUw5uXlZn8T9LVI5ZWI1jEQTxozkgECgkDrmKqfrFy8ILwJ7om+3bNoQumTRwtDoqE0fTBsf2ggwg+jVBdOCT7eYwGfnti2bQXA6ME2nr9mbnHLOWV/fEI3WTdO0jMzdZjBAKWBwX8ojCqm8vOJoYvLp9qPfHTmy5rXlJ+BSbtzI5+5EI4ALRCTHHHpaQ8zWqOidO2IooBAKRKRDQDwGevJ4w8SQUR0e0bmB0QxEKh2IYsdbTW0zmIxM4/Wi4q9BfQMkCikCoAEUADgEeI3xOOVedkicp14e1V2uLwSpTwxNAPwRaGC7OQFqQp9xGDT+1ksUUubFrMoLFy/VL5g7+4ep48fa+P0Pz9jnn4H7JCcQBbP79V1rgJDmASE9um7NqvmxMdFbVateiwd7KKswHx+dwBKwzGq1jgDRrjQ7W5sB6hvsRUhQQCyh8Sg4xwW64/oTpUQ/CIm7xz652yg9flb40R+xIn5i/LWJKKSk5NOuwqIi7cSQkXooAD6ywE8YneDyLWrDuq/WR67+BvxcB5dtG9dGHgF7oZsgSuWFz555c0LISKcwIvHlAHSdnR0P37h5699pzIW6NrNlptFoIglJ7cOAgcTf40711nH3g5AguEH3/4YGaZPSj/6Ix/hGmKd/hXQqIanz5q1b8WA5VwOXdLwgoIjAsk2/Y1v0odUrXj0OT+vgNSCkjgXzZleANF3wpI6PRALxcDDt7BlTby+NWPgdqOPBisrKz8E+zFFXX79Sp9fjhKQiDAqjx6kRHmfCdHDWZek+zCp+gnac6i7XhxOSUkAExiZI7D32y73wtbKfy/CnPDdEISUkJjsrKiqPhocp86ZPGGeDSzkIWJa1Rq5ccXyDas1X8PBBuG9Cow8UE/yEaYYPeZybPnFcM1gGRh/6+KNhNbV1o7Mua29dysrOdblcQ4SvDHmMg5s/I2ZAxNP+bQz5zaVaABz0ij7kh6D7NVJnwL1NLJLXn47DCQmXjkXSqAnpFB4/CO2KkODjEE861B9i7VcKwPldgaQJQfKi4yFWkNZbPXzZuP4iQRobaLrBIhEpubP0xq2E9989MHnLpg3rX5hFlz3/1BMcWLaVRm/eeIieNL4KRhi450EjDxQOvAf2T+mrli9bDZaAq3Zu37b3nbf2zvnwg/d/DoRENbcYRmhzcn84n5peDkQ0FbNHUmMGjD/LtsGesnCi5GEEnYbLH+clP9ox6ABiRdKzmDz9ISR0wKgx7WJE7ILtxUUxlQQfGDFtQutC7cH1OUPIi8NbPWjZUtBgbIzApFMQhZSccrbrav61zAqWfWR79JbJ8+eG5Q97/HccfB0I/P4eEJADRigoJP6NBvgzBC715s2coTuwf9+0qI3rKbB3ooCQKCAkCgiJgkKCS7uWFuMbiUkpjpzcvCvg9yGIkFicwZiGeRMR7oQPB+x8VEy+5OcRDiDcoCdBErI/QsINdmH5pGiPAxUT6cQLxYjkY5D7aozdaiQNQ8iLoz+EhPY1i7FRg7ORKKTUtHSdVptTarPZhr737oFHgRj+7lmeVcRsjfrwxdkzc+DSDj50VU6Z0LR5/drDK5a8HLt4QfhusAfaBUQz8tDHHw/atE5FEhLkods6/ZfHjsdzZWXlJwRCGoxppAbTKG+gjeadoyZ0Duo43MbU6LmuJpTPCwk3WGFHqTyg9xiJbcIJSS2AtJkWG9R89Imgew8mI91zmcfQPfeo/D21iC9wdUZg2oaWoaG7xYvm59vFQ6qHt0EloQycb4WTN25cuttBFBKIRpfAsstkNpvD4Xtye9/802PLFi/6J1y6LXpx3mUQleJARHKCaGRbvWLZO1AwQEgUEBIFhOQWDRAS5UVIFOfinrheVHw2MTmFEwgJ1yAVxvFiKDBlaJA0uJmbrycEcw+3P0PTCDtOeJ1F8uKWCFL2fr5EOZzNOL+g0Qq9Lxz0IQQ7ceUKhSR2jzRxqb2Uj/MP46Ueb2WwyH1hREaPzln+HlFIjY1N+1NSzlirq/Wfg99/9saunVRszLaHdu3YHg32PueAOP4Klm8lk0JHt4GfZ6yPXE0tf2WxZCHZ7Q7K4XC667I77IuZC5nehIRzvBhqJD86s/KgM7CG7p4FUafh8pPsRAeFhu69SfWnjTgBisEi5aKDoQBjl7f9FSqgWBq/FPdVSIxIvTh/+Sok3OSI5kf7XbgvR/1yR2REIXV0dIRmX9beys7WljsdzhEeIQFBxFDLXl5E7doRMzFs+pTG+XNmFX726acPHo6Loz45fJhasmihG29CstraqfZ2+wCXyzWCZau+T0w63d9CQgcy6aACdRxDcJqKkJ9kp9Q9iK9tVGPyqQXgDkbg7wqCX6SgRmyAdmpo7w/JAyEk1Calj2WgYjOKXL8zsRKFBKNQA4hKp8+c62poaPwjfI0HLOfcX4WAYoqO2jQKLPVSdr++azsUkK9CagdCstnah14rvJ767XdHHSUlN64IhISbOdDO9IZYp4gNTIbGd7wCk1ch0jHodf4VJjGkHDig9nKYNLCDWSQN/3YD6hdWgl38JOLtpA9FTEg4f6JlqwX3pAoJTRMiUgZDKAP1HcyHTrgaYR4xIVFOp/PJgmuFFfngf52dnU+Q0nkDLuOsVitlb293Cwhib7dTFotlWloaU3s1vyANpHsUObVDHcISGt1XIWkIzpXSabhlli8zsD+oJdpGirRS/YIDd4LJeurCTX68WKQsqXA+E9qG+ho9FSSVIbwnVUgajB1olO8xEYgKCdLaaoouKv6hrNXYOt9ut8PlGAF3hMGWAa83NjVRNpDG4XDcwWg0rklLZ7iS0hufgXQDESHhliBCx3oDdUYBIR1LqAOtGxct0DqEHYd7eHg3hMRKbD9D8KvUZ3MqTFuFbVKI+AIdwDh/4soXTj5ouxkabyfJBl+E5G0f2isfUUjwD5RAzGbzQzW1dXOqdbphNbW1VE0NHp1OD6KOTVRI7UCIgusP6Gtq9iWnnOmqul0dhXkgi3M+BM5+pNOtELp7pvDWMRDcC4x8B6OzLzrgcLOssOPQAcuK2N0XIfXqVI9tqJB5+8Xa7Eu96IuwuP4Suyf0J85ejhYX0t2MSBTBHh4Vmp4opJYWgxujsZWqr2+ggJAoXY2eAoO/F/Ce1YYXkVBIMKKB5SJc0sGl3rC8/ALt2fNpzQ6HM9zVW0i4WVXoRP5ZjprufrbB0d0RBfccx0h3v8aCK1voWLTjOE+d/GsxJEeLzbAFdPdRMv/KUSwtfX+Es4ulex42kHzGd74Cc8/ouc8LXen5PV6QD62XEaRXENrrbVI00uIPvMWExHl8F0/37DeSDb4KieRHFpeeKCSDwegGCqmurt4tFn9E1CMigaWd52/jQX5fUlqakprOmMB/LzU3N+OEJNYgKc735agYfbPBl6f/pI5jfMgnNVr5UiYPuqxV+5CXFz4uAguFgFuKS53hSQj7UuzrD3x09LYXQ9vN0GQ/k8aOGpe+T0K6XV1NWaxWKYcNA1sMhgdANHLvgzo7u9zXK1n20PnzaVYQ8ZbB5SFBSPzszkp0vgLjEG+dyNL4iEBacvBovHQcFIeU42ZWpEP7KiTSS75qifmF/sS1lwc30H3pB1xkEgpJIZKfj5q4yOevkEjix054fgsJfu0BwkcZEqCs3zQ2Ne8pLin5urpad8hkaltQUnLjGbDfimQyLhjg298gDe7tb9Isoabx3wRV0/jXTvgBrfKkE+aLE8kjzCtcQvD5FB7UCLgyQgh288tTJSEfaVJB68QRQXt/N1GBaRuPmsY/OyP5UYov+DTCvBq65/JRCGq/AlM3tF+4xBSzQYncw7VPCOlhff8ICQqotq7OfRghWKphMZstaxKTUywnTp5qPHP2vOn0mXNcKpNhPpWYxKWmpjeDZd0WtG4vjZORuRcoafEI2QO/hASXdAajUcozpEGF14uPpgPhWK22xRaLdUbV7eo3b9ws28+yVXsdDvtceHonC0nmPoShey89ien9jkjNLQaqrc1MxASw2donpaZn1JeVlyeBfdEv2232O/sjMe4DJ8r8+GDo7i8K4va1KrH8PgsJPkuC+yL4tgL8JAGPucvKK2MzM7PaWltbl4AyB/wvj10Wksz9CCeCaDSC+CQkGInq6utF90Q8oIzf5l0tuFheXvkPsI962HN6JwtJ5n6FofEiwn3hsxeShVQF9kVQRPDfSZKwN6Kampt3Xiu83mQymcL5a/BrE1BMspBk7kNUdO8TVeGJoCiShOR+DaiuTvKfFQbpHqmoqMzW6/WJ8PgbOQ6XkQlKsBd5IUFaDAbJkQhitdpWgKUg226zLYS/y0KS+TGAvdjc3OKmqamFamtroywWq+gpHY/ZbBnU3GL4FHx+A8r5BeEhrYxM0BFwA2RkgoGAGyAjEwwE3AAZmWAg4AbIyAQDATdARiYYCLgBMjLBQMANkJEJBgJugIxMMPBfChd6NRZ5pkMAAAAASUVORK5CYII=";
        public Task<IEnumerable<Product>> GetUnapprovedAsync(bool forceRefresh)
        {
            throw new NotImplementedException();
        }

        Task<Product> IProductService.AddAsync(Product item)
        {
            items.Add(item);

            return Task.FromResult(item);
        }

        Task<Product> IProductService.UpdateAsync(Product item)
        {
            var _item = items.Where((Product arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(_item);
            items.Add(item);

            return Task.FromResult(item);
        }

        Task IProductService.DeleteAsync(Product item)
        {
            var _item = items.Where((Product arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(_item);

            return Task.CompletedTask;
        }

        async Task<PagedList<Product>> IProductService.AllAsync(int page = 1, int pageSize = 10, bool forceRefresh = false, bool includeUnapproved = false)
        {
            await Task.Delay(2000);
            page--;

            if (page * pageSize >= items.Count)
            {
                return null;
            }

            IEnumerable<Product> result = items.Skip(page * pageSize).Take(pageSize);
            return new PagedList<Product> { Items = result };
        }

        Task<Product> IProductService.GetAsync(string id)
        {
            return Task.FromResult(items.First(p => p.Id == id));
        }
    }
}

