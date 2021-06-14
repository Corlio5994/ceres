using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

public class VectorJsonConverter : JsonConverter<Vector3> {
    public override Vector3 Read (ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException ("Expected StartObject token");

        var result = new Vector3 ();

        while (reader.Read ()) {
            if (reader.TokenType == JsonTokenType.EndObject)
                return result;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException ("Expected PropertyName token");

            var propertyName = reader.GetString ();
            reader.Read ();
            float number = reader.GetSingle();

            switch (propertyName) {
                case "x":
                    result.x = number;
                    break;
                case "y":
                    result.y = number;
                    break;
                case "z":
                    result.z = number;
                    break;
            }
        }

        throw new JsonException ("Expected EndObject token");
    }

    public override void Write (Utf8JsonWriter writer, Vector3 vector, JsonSerializerOptions options) {
        writer.WriteStartObject ();

        writer.WriteNumber ("x", vector.x);
        writer.WriteNumber ("y", vector.y);
        writer.WriteNumber ("z", vector.z);

        writer.WriteEndObject ();
    }
}