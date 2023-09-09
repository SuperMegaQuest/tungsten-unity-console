using System;

using Newtonsoft.Json;

using UnityEngine;

namespace Monolith.Web {

public class ColorJsonConverter : JsonConverter {

    public override bool CanConvert(Type objectType) {
        return objectType == typeof(Color);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
        Color color = (Color)value;
        string colorString = $"#{ColorUtility.ToHtmlStringRGBA(color)}";
        writer.WriteValue(colorString);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
        if (reader.Value == null) {
            return null;
        }

        string colorString = (string)reader.Value;
        ColorUtility.TryParseHtmlString(colorString, out Color color);
        return color;
    }

}

}