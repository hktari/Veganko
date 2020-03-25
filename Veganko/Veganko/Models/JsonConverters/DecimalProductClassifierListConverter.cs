using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Veganko.Common.Models.Products;

namespace Veganko.Models.JsonConverters
{
    public class DecimalProductClassifierListConverter : JsonConverter
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
            IList<ProductClassifier> classifiers = new List<ProductClassifier>();
            int curFlag = 1;
            while (flag > 0)
            {
                if (flag % 2 != 0)
                    classifiers.Add((ProductClassifier)curFlag);
                flag /= 2;
                curFlag *= 2;
            }
            return classifiers;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IList<ProductClassifier> classifiers = value as IList<ProductClassifier>;
            if (classifiers == null)
                throw new ArgumentException($"Invalid value type ! Value is {value?.GetType()}", "value");
            int flag = 0;
            foreach (var classifier in classifiers)
                flag += (int)classifier;
            writer.WriteValue(flag);
        }
    }
}
