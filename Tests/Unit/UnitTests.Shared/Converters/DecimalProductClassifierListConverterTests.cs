using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Common.Models.Products;
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
                Converters = new List<JsonConverter> { new DecimalProductClassifierListJsonConverter() }
            };

            string json = JsonConvert.SerializeObject(classifiers, jsonSerializerSettings);
            IList<ProductClassifier> deserialized = JsonConvert.DeserializeObject<IList<ProductClassifier>>(json, jsonSerializerSettings);
            for (int i = 0; i < classifiers.Count; i++)
            {
                Assert.AreEqual(classifiers[i], deserialized[classifiers.Count - i - 1]);
            }
        }

        [TestMethod]
        public void SerializationAndDeserializationOfDefaultValue_ResultsInEmpty()
        {
            List<ProductClassifier> classifiers = new List<ProductClassifier>
            {
                default,
            };

            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new DecimalProductClassifierListJsonConverter() }
            };

            string json = JsonConvert.SerializeObject(classifiers, jsonSerializerSettings);
            IList<ProductClassifier> deserialized = JsonConvert.DeserializeObject<IList<ProductClassifier>>(json, jsonSerializerSettings);
            Assert.AreEqual(0, deserialized.Count);
        }

        [TestMethod]
        public void SerializationAndDeserializationOfEmptyList_ResultsInEmptyList()
        {
            List<ProductClassifier> classifiers = new List<ProductClassifier>
            {
            };

            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new DecimalProductClassifierListJsonConverter() }
            };

            string json = JsonConvert.SerializeObject(classifiers, jsonSerializerSettings);
            IList<ProductClassifier> deserialized = JsonConvert.DeserializeObject<IList<ProductClassifier>>(json, jsonSerializerSettings);
            Assert.AreEqual(0, deserialized.Count);
        }
    }
}
