using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Validations;

namespace UnitTests.Shared.Validations
{
    [TestClass]
    public class NoInvalidCharactersRuleTests
    {
        [TestMethod]
        public void TestCheck()
        {
            var rule = new NoInvalidCharactersRule("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

            string[] valid = new[] { "myname", "myname123", "2123", "0092kals" };

            for (int i = 0; i < valid.Length; i++)
            {
                Assert.IsTrue(rule.Check(valid[i]));
            }

            string[] invalid = new[] { "myname123!", "hello_test", "pops$", "my name", "  names be like this", "my.name" };

            for (int i = 0; i < invalid.Length; i++)
            {
                Assert.IsFalse(rule.Check(invalid[i]));
            }
        }
    }
}
