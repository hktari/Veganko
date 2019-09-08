using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Veganko.Models.JsonConverters
{
    public class Base64StringToByteArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var base64Str = reader.Value as string;
            if (base64Str == null)
            {
                return null;
            }

            return Convert.FromBase64String(base64Str);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(
                Convert.ToBase64String((byte[])value));
        }
    }
}
