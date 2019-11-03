using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Veganko.Validations
{
    public class IsValidEmailRule : IValidationRule<string>
    {
        public string ValidationMessage { get; set; }

        public bool Check(string value)
        {
            try
            {
                _ = new MailAddress(value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
