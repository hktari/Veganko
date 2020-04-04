using System;
using System.Collections.Generic;
using Veganko.Common.Models.Products;
using VegankoService.Data;
using VegankoService.Models.User;

namespace VegankoService.Tests
{
    public static class Util
    {
        public static void InitializeDbForTests(VegankoContext db)
        {
            db.Product.AddRange(GetProducts());

            AddProductModRequests(db);

            db.SaveChanges();
        }

        private static void AddProductModRequests(VegankoContext db)
        {
            List<Product> products = new List<Product>
            {
                new Product { Id = "existing_product_id", Name = "Hot Chilli almonds", ProductClassifiers = 512, Type = "FOOD", },
                new Product { Id = "existing_product_two_id", Name = "Chilli almonds", ProductClassifiers = 512, Type = "FOOD", Barcode = "conflicting_barcode" },
            };
            db.Product.AddRange(products);

            db.SaveChanges();

            List<UnapprovedProduct> unapprovedProducts = new List<UnapprovedProduct>
            {
                new UnapprovedProduct
                {
                    Id = "edit_existing_product_id",
                    ProductClassifiers = 256,
                    Type = "BEVERAGE",
                },
                new UnapprovedProduct
                {
                    Id = "new_unprvd_product",
                    Name = "Peanut butter",
                    Brand = "Rok's",
                    Barcode = "1234"
                },
                new UnapprovedProduct
                {
                    Id = "unprvd_product",
                    Name = "Chocolate Bananas",
                    Barcode = "123",
                    ProductClassifiers = 512,
                },
                new UnapprovedProduct
                {
                    Id = "existing_prod_id",
                    Name = "Chocolate Bananas",
                    Barcode = "123",
                    ProductClassifiers = 512,
                },
                new UnapprovedProduct
                {
                    Id = "rejected_prod_id",
                    Name = "Chocolate Bananas",
                    Barcode = "123",
                    ProductClassifiers = 512,
                }
            };

            db.UnapprovedProducts.AddRange(unapprovedProducts);
            db.SaveChanges();

            List<ProductModRequest> productModRequests = new List<ProductModRequest>
            {
                new ProductModRequest
                {
                    Id = "edit_prod_mod_req_id",
                    UserId = "user_id",
                    ExistingProductId = "existing_product_id",
                    Action = ProductModRequestAction.Edit,
                    ChangedFields = "PRODUCTCLASSIFIERS, TYPE",
                    Timestamp = DateTime.Now,
                    UnapprovedProduct = unapprovedProducts[0],
                    State = ProductModRequestState.Pending,
                },
                new ProductModRequest
                {
                    Id = "new_prod_mod_req_id",
                    UserId = "user_id",
                    ExistingProductId = null,
                    Action = ProductModRequestAction.Add,
                    ChangedFields = null,
                    Timestamp = DateTime.Now,
                    UnapprovedProduct = unapprovedProducts[1],
                    State = ProductModRequestState.Pending,
                },
                new ProductModRequest
                {
                    Id = "other_user_prod_mod_req_id",
                    UserId = "other_user_id",
                    ExistingProductId = null,
                    Action = ProductModRequestAction.Add,
                    ChangedFields = null,
                    Timestamp = DateTime.Now,
                    UnapprovedProduct = unapprovedProducts[2],
                    State = ProductModRequestState.Pending,
                },
                new ProductModRequest
                {
                    Id = "existing_prod_mod_req_id",
                    UserId = "user_id",
                    ExistingProductId = null,
                    Action = ProductModRequestAction.Add,
                    ChangedFields = null,
                    Timestamp = DateTime.Now,
                    UnapprovedProduct = unapprovedProducts[3],
                    State = ProductModRequestState.Pending,
                },
                new ProductModRequest
                {
                    Id = "rejected_prod_mod_req_id",
                    UserId = "user_id",
                    ExistingProductId = null,
                    Action = ProductModRequestAction.Add,
                    ChangedFields = null,
                    Timestamp = DateTime.Now,
                    UnapprovedProduct = unapprovedProducts[3],
                    State = ProductModRequestState.Rejected,
                }
            };

            db.ProductModRequests.AddRange(productModRequests);

            db.Customer.AddRange(new[]
            {
                new Customer { Id = "user_id", IdentityId = "user_id" }
            });

            db.SaveChanges();
        }

        public static void ReinitializeDbForTests(VegankoContext db)
        {
            db.Database.EnsureDeleted();
            //db.Product.RemoveRange(db.Product);
            //db.ProductModRequests.RemoveRange(db.ProductModRequests);

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
