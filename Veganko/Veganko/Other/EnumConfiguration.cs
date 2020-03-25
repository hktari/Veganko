using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veganko.Common.Models.Products;
using Veganko.Models;

namespace Veganko.Other
{
    public static class EnumConfiguration
    {
        public static Dictionary<ProductType, List<ProductClassifier>> ClassifierDictionary => new Dictionary<ProductType, List<ProductClassifier>>
        {
            {
                ProductType.NOT_SET, new List<ProductClassifier>()
            },
            {
                ProductType.Ostalo,
                GetAllProdClassifiers()
            },
            {
                ProductType.Hrana, new List<ProductClassifier>
                {
                    ProductClassifier.Vegansko,
                    ProductClassifier.GlutenFree,
                    ProductClassifier.RawVegan,
                    ProductClassifier.Bio,
                    ProductClassifier.SoyFree,
                    ProductClassifier.NutFree
                }
            },
            {
                ProductType.Pijaca, new List<ProductClassifier>
                {
                    ProductClassifier.Vegansko,
                    ProductClassifier.GlutenFree,
                    ProductClassifier.RawVegan,
                    ProductClassifier.Bio,
                    ProductClassifier.SoyFree,
                    ProductClassifier.NutFree
                }
            },
            {
                ProductType.Kozmetika, new List<ProductClassifier>
                {
                    ProductClassifier.Vegansko,
                    ProductClassifier.CrueltyFree,
                    ProductClassifier.Bio
                }
            }
        };

        /// <summary>
        /// Returns a list of all <see cref="ProductClassifier"/> excluding the first value.
        /// </summary>
        /// <returns></returns>
        private static List<ProductClassifier> GetAllProdClassifiers()
        {
            return ((IEnumerable<ProductClassifier>)(Enum.GetValues(typeof(ProductClassifier)))).Skip(1)
                                                                                                .ToList();
        }
    }
}
