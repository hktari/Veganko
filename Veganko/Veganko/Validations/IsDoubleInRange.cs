using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Validations
{
    public class IsDoubleInRange : IValidationRule<double>
    {
        public string ValidationMessage { get; set; }

        public double? Minimum { get; }

        public double? Maximum { get; }

        /// <param name="min">The included minimum allowed value.</param>
        /// <param name="max">The excluded maximum allowed value.</param>
        public IsDoubleInRange(double? min = null, double? max = null)
        {
            if(min == null && max == null)
            {
                throw new ArgumentException("Both can't be null.");
            }

            Minimum = min;
            Maximum = max;
        }

        public bool Check(double value)
        {
            bool isInRange = false;

            if (Minimum != null)
            {
                isInRange = value >= Minimum;
            }

            if (Maximum != null)
            {
                isInRange &= value < Maximum;
            }

            return isInRange;
        }
    }
}
