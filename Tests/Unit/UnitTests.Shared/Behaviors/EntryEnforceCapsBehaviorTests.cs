using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Behaviors;
using Xamarin.Forms;

namespace UnitTests.Shared.Behaviors
{
    [TestClass]
    public class EntryEnforceCapsBehaviorTests
    {
        [TestMethod]
        public void TestOnTextChanged()
        {
            var entryBehav = new EntryEnforceCapsBehavior();
            Entry entry = new Entry();
            entry.Behaviors.Add(entryBehav);

            entry.Text = "test";

            Assert.AreEqual("TEST", entry.Text);
        }
    }
}
