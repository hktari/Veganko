using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using Veganko.Common.Models.Products;
using Veganko.Models;
using Veganko.Models.Products;

namespace Veganko.Services.DB
{
    public class ProductDBService : IProductDBService
    {
        private const string DbName = "data.db3";

        public const SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLiteOpenFlags.SharedCache;
        
        public static string DatabasePath
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DbName);
            }
        }

        private static readonly Lazy<SQLiteConnection> dbConnection = new Lazy<SQLiteConnection>(
            () => new SQLiteConnection(DatabasePath, Flags));

        private static SQLiteConnection Database => dbConnection.Value;

        private static bool initialized;

        public ProductDBService()
        {
            Initialize();
        }

        public Task<IEnumerable<CachedProduct>> GetAllSeenProducts()
        {
            var products = Database.Table<CachedProduct>().Where(cp => cp.HasBeenSeen).ToList();

            return Task.FromResult(
                products.AsEnumerable());
        }

        //public Task SetAsSeen(CachedProduct product)
        //{
        //    Database.Inser
        //}

        private void Initialize()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(CachedProduct).Name))
                {
                    Database.CreateTables(CreateFlags.None, typeof(CachedProduct));
                    initialized = true;
                }
            }
        }

        public Task SetProductsAsSeen(IEnumerable<Product> products)
        {
            List<CachedProduct> toUpdate = new List<CachedProduct>();
            List<CachedProduct> toInsert = new List<CachedProduct>();
            foreach (var product in products)
            {
                CachedProduct cache = Database.Table<CachedProduct>().FirstOrDefault(cp => cp.ProductId == product.Id);
                bool update = true;
                if (cache == null)
                {
                    update = false;
                    cache = new CachedProduct(product);
                }
                
                cache.HasBeenSeen = true;
                cache.LastSeenTimestamp = DateTime.Now;

                if (update)
                {
                    toUpdate.Add(cache);
                }
                else 
                {
                    toInsert.Add(cache);
                }
            }

            // Avoid Sqlite Exception "no such savepoint: S6128D0"
            if (toUpdate.Count > 0)
            {
                int cnt = Database.UpdateAll(toUpdate);
                Debug.WriteLine($"DB Service: updated: {cnt} entries.");
            }

            // Avoid Sqlite Exception "no such savepoint: S6128D0"
            if (toInsert.Count > 0)
            {
                int cnt = Database.InsertAll(toInsert);
                Debug.WriteLine($"DB Service: inserted: {cnt} entries.");
            }

            return Task.CompletedTask;

            ////IEnumerable<CachedProduct> cachedProds = Database.Table<CachedProduct>()

            ////List<Product> toAdd = products.ToList();
            ////foreach (var prod in cachedProds)
            ////{
            ////    prod.HasBeenSeen = true;

            ////}

            //// All unseen products which have to be set as seen
            //IEnumerable<Tuple<CachedProduct, Product>> unseenProds = 
            //    Database.Table<CachedProduct>()
            //    .Join(products, cp => cp.ProductId, prod => prod.Id, (cp, prod) => Tuple.Create(cp, prod))
            //    .Where(pp => !pp.Item1.HasBeenSeen);

            //// All products which don't exist in the cache.
            //IEnumerable<Product> justSeenProds = products.Except(
            //    unseenProds.Select(pp => pp.Item2));

            //Database.U

            //foreach (var prod in collection)
            //{

            //}
            //unseenProds.Select(pp => pp.Item1)
        }
    }
}
