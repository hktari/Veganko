using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Common.Models.Products;
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
            { ProductClassifier.Vegeterijansko, "ico_vegetarian.png" },
            { ProductClassifier.Vegansko, "ico_vegan.png" },
            { ProductClassifier.Pesketarijansko, "ico_pescetarian.png" },
            { ProductClassifier.GlutenFree, "ico_gluten_free.png" },
            { ProductClassifier.RawVegan, "ico_raw_vegan.png" },
            { ProductClassifier.CrueltyFree, "ico_cruelty_free.png" },
            { ProductClassifier.Bio, "ico_bio.png" },
            { ProductClassifier.SoyFree, "ico_soy_free.png" },
            { ProductClassifier.NutFree, "ico_nut_free.png" },
        };

        public static Dictionary<ProductType, string> ProductTypeImages => new Dictionary<ProductType, string>
        {
            { ProductType.Hrana, "ico_food.png" },
            { ProductType.Pijaca, "ico_beverages.png" },
            { ProductType.Kozmetika, "ico_cosmetics.png" }
        };

        public static Dictionary<string, string> ProfileBackgroundImages => new Dictionary<string, string>
        {
            { "0", "pbg_1.png" },
            { "1", "pbg_2.png" },
            { "2", "pbg_3.png" },
            { "3", "pbg_4.png" },
            { "4", "pbg_5.png" },
            { "5", "pbg_6.png" },
        };
    }
}
