using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Models;

namespace Veganko
{
    public static class EnumImages
    {
        //public static Dictionary<ProductClassifier, string> ProductClassifierImages => new Dictionary<ProductClassifier, string>
        //{
        //    { ProductClassifier.Vegeterijansko, "ico_vegetarian.png" },
        //    { ProductClassifier.Vegansko, "vegan.png" },
        //    { ProductClassifier.Pesketarijansko, "ico_pescetarian.png" },
        //    { ProductClassifier.GlutenFree, "ico_gluten_free.png" },
        //    { ProductClassifier.RawVegan, "ico_raw_vegan.png" },
        //    { ProductClassifier.CrueltyFree, "ico_cruelty_free.png" },
        //};
        public static Dictionary<ProductClassifier, string> ProductClassifierImages => new Dictionary<ProductClassifier, string>
        {
            { ProductClassifier.Vegeterijansko, "ico_vegan.png" },
            { ProductClassifier.Vegansko, "ico_vegan.png" },
            { ProductClassifier.Pesketarijansko, "ico_vegan.png" },
            { ProductClassifier.GlutenFree, "ico_vegan.png" },
            { ProductClassifier.RawVegan, "ico_vegan.png" },
            { ProductClassifier.CrueltyFree, "ico_vegan.png" },
        };

        public static Dictionary<ProductType, string> ProductTypeImages => new Dictionary<ProductType, string>
        {
            { ProductType.Hrana, "ico_food.png" },
            { ProductType.Pijaca, "ico_beverages.png" },
            { ProductType.Kozmetika, "ico_cosmetics.png" }
        };
    }
}
