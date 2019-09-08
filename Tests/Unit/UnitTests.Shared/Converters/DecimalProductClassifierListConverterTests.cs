using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Models;
using Veganko.Models.JsonConverters;

namespace UnitTests.Shared.Converters
{
    [TestClass]
    public class DecimalProductClassifierListConverterTests
    {
        [TestMethod]
        public void Expect_SameValueAfterSerializationAndDeserialization()
        {
            List<ProductClassifier> classifiers = new List<ProductClassifier>
            {
                ProductClassifier.Bio,
                ProductClassifier.Vegansko,
            };

            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new DecimalProductClassifierListConverter() }
            };

            string json = JsonConvert.SerializeObject(classifiers, jsonSerializerSettings);
            Assert.AreEqual(classifiers, JsonConvert.DeserializeObject(json, jsonSerializerSettings));
        }
    }
}
