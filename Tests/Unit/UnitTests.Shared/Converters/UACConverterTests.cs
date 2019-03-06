using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Converters;
using Veganko.Models.User;

namespace UnitTests.Shared.Converters
{
    [TestClass]
    public class UACConverterTests
    {
        private UACConverter converter = new UACConverter();

        [TestMethod]
        public void TestConvertMatchingMask()
        {
            var mask = UserAccessRights.ProductsDelete;
            var uac = UserAccessRights.All;
            
            Assert.IsTrue((bool)converter.Convert(uac, typeof(bool), mask, null));
        }

        [TestMethod]
        public void TestConvertNonMatchingMask()
        {
            var mask = UserAccessRights.ProductsDelete;
            var uac = UserAccessRights.ProductsRead | UserAccessRights.ProductsWrite;

            Assert.IsFalse((bool)converter.Convert(uac, typeof(bool), mask, null));
        }
    }
}
