using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hawf.Json.Converters;

public class StringToIntArrayConverter : JsonConverter<int[]>
{
    public override int[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString()?.Split(',').Select(int.Parse).ToArray();
    }

    public override void Write(Utf8JsonWriter writer, int[] value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(string.Join(',', value));
    }
}