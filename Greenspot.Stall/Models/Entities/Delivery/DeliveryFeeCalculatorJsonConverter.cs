using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;

namespace Greenspot.Stall.Models
{
    public class DeliveryFeeCalculatorJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(DeliveryFeeCalculator));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            var type = jo["Type"].Value<string>();
            switch (type)
            {
                case "ByRange": return jo.ToObject<ByRangeCalculator>(serializer);
                case "Fixed": return jo.ToObject<ByRangeCalculator>(serializer);
                default: return null;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
