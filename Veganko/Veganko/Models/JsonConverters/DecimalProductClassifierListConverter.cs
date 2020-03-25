using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Veganko.Common.Models.Products;

namespace Veganko.Models.JsonConverters
{
    public class DecimalProductClassifierListJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IList<ProductClassifier>).IsAssignableFrom(objectType);
        }
        public override bool CanRead => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;
            int? flag = int.Parse(reader.Value.ToString());
            if (flag == null)
                throw new ArgumentException($"Invalid flag value '{reader.Value}'", "reader.Value");

            IList<ProductClassifier> classifiers = new Veganko.Other.DecimalProductClassifierListConverter().Convert(flag.Value);
            return classifiers;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IList<ProductClassifier> classifiers = value as IList<ProductClassifier>;
            if (classifiers == null)
                throw new ArgumentException($"Invalid value type ! Value is {value?.GetType()}", "value");
            int flag = new Veganko.Other.DecimalProductClassifierListConverter().ConvertBack(classifiers);
            writer.WriteValue(flag);
        }
    }
}
