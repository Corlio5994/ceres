#if UNITY_SERVER || UNITY_EDITOR
using UnityEngine;
using Newtonsoft.Json;
using System;


public class VectorJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        if (objectType == typeof(Vector3))
        {
            return true;
        }
        return false;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var t = serializer.Deserialize(reader);
        var iv = JsonConvert.DeserializeObject<Vector3>(t.ToString());
        return iv;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Vector3 v = (Vector3)value;

        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteValue(v.x);
        writer.WritePropertyName("y");
        writer.WriteValue(v.y);
        writer.WritePropertyName("z");
        writer.WriteValue(v.z);
        writer.WriteEndObject();
    }
}
#endif