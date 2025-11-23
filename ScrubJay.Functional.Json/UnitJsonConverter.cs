using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScrubJay.Functional.Json;

public sealed class UnitJsonConverter : JsonConverter<Unit>
{
    public override Unit Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException();
        if (reader.ValueSpan.SequenceEqual("()"u8))
            return default;
        throw new JsonException();
    }
    
    public override void Write(Utf8JsonWriter writer, Unit _, JsonSerializerOptions options)
    {
        writer.WriteStringValue("()");
    }
}