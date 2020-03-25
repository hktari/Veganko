using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Common.Models.Products;

namespace Veganko.Other
{
    public class DecimalProductClassifierListConverter
    {
        public IList<ProductClassifier> Convert(int mask)
        {
            var classifiers = new List<ProductClassifier>();
            int curFlag = 1;
            while (mask > 0)
            {
                if (mask % 2 != 0)
                    classifiers.Add((ProductClassifier)curFlag);
                mask /= 2;
                curFlag *= 2;
            }

            return classifiers;
        }

        public int ConvertBack(IList<ProductClassifier> classifiers)
        {
            int mask = 0;
            foreach (var classifier in classifiers)
                mask += (int)classifier;
            return mask;
        }
    }
}
