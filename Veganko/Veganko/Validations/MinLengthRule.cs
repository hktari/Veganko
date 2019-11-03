using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Validations
{
    public class MinLengthRule : IValidationRule<string>
    {
        private readonly int minLength;

        public MinLengthRule(int minLength)
        {
            this.minLength = minLength;

            ValidationMessage = $"Nepravilna dolžina. Vsebovati mora najmanj {minLength} znakov.";
        }

        public string ValidationMessage { get; set; } 

        public bool Check(string value)
        {
            if (value == null)
            {
                return false;
            }

            return value.Length >= minLength;
        }
    }
}
