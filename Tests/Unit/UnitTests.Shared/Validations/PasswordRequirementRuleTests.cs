using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Validations;

namespace UnitTests.Shared.Validations
{
    [TestClass]
    public class PasswordRequirementRuleTests
    {
        [TestMethod]
        public void CheckPasswordsWithAllRequirements()
        {
            PasswordRequirementRule rule = new PasswordRequirementRule(true, true, true, true);

            string[] validPwds = new string[]
            {
                "TestpwD1*", "_testPwd1", "10TESTpwd*", "923$*?ksdlaO", @"[!@#$%^&*(),.?"":{ }|<>]dK999"
            };
            string[] invalidPwds = new string[]
            {
                "test", "", null, "TEST", "Test123", "123", "**", "Test??*", @"[!@#$%^&*(),.?"":{ }|<>]"
            };
        
            CheckPasswords(rule, validPwds, invalidPwds);
        }

		[TestMethod]
		public void CheckPasswordsWithAlphaNumericRequirement()
		{
			PasswordRequirementRule rule = new PasswordRequirementRule(true, false, true, false);

            string[] validPwds = new string[]
            {
                "test123", "Test123", "TEsT123"
            };
            string[] invalidPwds = new string[] 
            {
                "test", "TEST", "123" 
            };

			CheckPasswords(rule, validPwds, invalidPwds);
		}

		private void CheckPasswords(PasswordRequirementRule rule, string[] validPwds, string[] invalidPwds)
		{
			foreach (var pwd in validPwds)
			{
				Assert.IsTrue(rule.Check(pwd));
			}

			foreach (var pwd in invalidPwds)
			{
				Assert.IsFalse(rule.Check(pwd));
			}
		}
	}
}
