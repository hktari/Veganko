using System.Collections.Generic;
using System.IO;
using VegankoService.Data;
using VegankoService.Models;
using VegankoService.Models.User;

namespace VegankoService.Tests
{
    public static class Utilities
    {
        public static void InitializeDbForTests(VegankoContext db)
        {
            db.Product.AddRange(GetProducts());
            db.SaveChanges();
        }

        public static void ReinitializeDbForTests(VegankoContext db)
        {
            db.Product.RemoveRange(db.Product);
            InitializeDbForTests(db);
        }

        public static List<Product> GetProducts()
        {
            return new List<Product> 
            {
                new Product { Name = "Bananas" },
                new Product { Name = "Chocolate cream" },
                new Product { Name = "Soap" },
                new Product { Name = "Tea" },
                new Product { Name = "Apple pie" },
                new Product { Name = "Sweater" },
                new Product { Name = "Apples" },
                new Product { Name = "Toothpaste" },
                new Product { Name = "Figs" },
            };    
        }

        //public static void AddUsers(VegankoContext db)
        //{ 
        //    //ApplicationUser user = new ApplicationUser { Id = "00", Email = "manager@email.com", UserName  = "manager", }
        //}

        public static string GetRequestUri(string resource)
        {
            return $"api/{resource}";
        }
    }
}
