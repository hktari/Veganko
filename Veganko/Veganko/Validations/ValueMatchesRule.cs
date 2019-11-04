using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Validations
{
    public class ValueMatchesRule : IValidationRule<string>
    {
        private readonly ValidatableObject<string> objectToMatch;

        public ValueMatchesRule(ValidatableObject<string> objectToMatch)
        {
            this.objectToMatch = objectToMatch;
        }

        public string ValidationMessage { get; set; }

        public bool Check(string value)
        {
            if (value == null)
            {
                return false;
            }

            return value.Equals(objectToMatch.Value);
        }
    }
}
