using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
