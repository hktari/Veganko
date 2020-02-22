using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Veganko.Validations
{
    public class NoInvalidCharactersRule : IValidationRule<string>
    {
        private readonly string validCharacters;

        public NoInvalidCharactersRule(string validCharacters)
        {
            this.validCharacters = validCharacters;
            ValidationMessage = "Nesme vsebovati presledkov. Podprti so naslednji znaki: " + validCharacters;
        }

        public string ValidationMessage { get; set; }

        public bool Check(string value)
        {
            if(value == null)
            {
                return false;
            }

            // No invalid characters've been found
            return !Regex.IsMatch(value, $"[^{validCharacters}]");
        }
    }
}
