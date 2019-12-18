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
        private Entry entry;

        [TestInitialize]
        public void Init()
        {
            var entryBehav = new EntryEnforceCapsBehavior();
            entry = new Entry();
            entry.Behaviors.Add(entryBehav);
        }

        [TestMethod]
        public void TestOnTextChanged()
        {
            entry.Text = "test";

            Assert.AreEqual("TEST", entry.Text);
        }

        [TestMethod]
        public void TestOnTextChangedToNull()
        {
            entry.Text = "text";
            entry.Text = null;

            Assert.AreEqual(null, entry.Text);
        }
    }
}
