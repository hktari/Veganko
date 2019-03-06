using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Shared
{
    [TestClass]
    public class Configuration
    {
        [AssemblyInitialize]
        public static void Init(TestContext testContext)
        {
            Xamarin.Forms.Mocks.MockForms.Init();
        }
    }
}
