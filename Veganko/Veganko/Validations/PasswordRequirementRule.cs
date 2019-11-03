using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Veganko.Validations
{
    public class PasswordRequirementRule : IValidationRule<string>
    {
        private readonly bool requireLowerCase;
        private readonly bool requireUpperCase;
        private readonly bool requireNumeric;
        private readonly bool requireSpecialChar;

        public PasswordRequirementRule(
            bool requireLowerCase, bool requireUpperCase, bool requireNumeric, bool requireSpecialChar)
        {
            Debug.Assert(requireLowerCase || requireUpperCase || requireNumeric || requireSpecialChar);

            ValidationMessage = "Mora vsebovati vsaj ";
            if (requireLowerCase)
            {
                ValidationMessage += "eno malo črko";
            }

            if (requireUpperCase)
            {
                AddCommaIfNeeded(requireLowerCase);
                ValidationMessage += "eno veliko črko";
            }

            if (requireNumeric)
            {
                AddCommaIfNeeded(requireLowerCase || requireUpperCase);
                ValidationMessage += "eno številko";
            }

            if (requireSpecialChar)
            {
                AddCommaIfNeeded(requireLowerCase || requireUpperCase || requireNumeric);
                ValidationMessage += "en poseben znak (npr. *, -, _)";
            }

            this.requireLowerCase = requireLowerCase;
            this.requireUpperCase = requireUpperCase;
            this.requireNumeric = requireNumeric;
            this.requireSpecialChar = requireSpecialChar;
        }

        private void AddCommaIfNeeded(bool shouldAddComma)
        {
            if (shouldAddComma)
            {
                ValidationMessage += ", ";
            }
        }

        public string ValidationMessage { get; set; }

        public bool Check(string value)
        {
            if (value == null)
            {
                return false;
            }

            bool requirementsMet = true;

            if (requireLowerCase)
            {
                requirementsMet &= Regex.IsMatch(value, "[a-z]");    
            }

            if (requireUpperCase)
            {
                requirementsMet &= Regex.IsMatch(value, "[A-Z]");
            }

            if (requireNumeric)
            {
                requirementsMet &= Regex.IsMatch(value, "[0-9]");
            }

            if (requireSpecialChar)
            {
                requirementsMet &= Regex.IsMatch(value, @"[^a-zA-Z\d\s:]");
            }

            return requirementsMet;
        }
    }
}
