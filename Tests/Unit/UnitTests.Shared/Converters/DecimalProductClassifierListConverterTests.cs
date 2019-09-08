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
            IList<ProductClassifier> deserialized = JsonConvert.DeserializeObject<IList<ProductClassifier>>(json, jsonSerializerSettings);
            for (int i = 0; i < classifiers.Count; i++)
            {
                Assert.AreEqual(classifiers[i], deserialized[classifiers.Count - i - 1]);
            }
        }
    }
}
