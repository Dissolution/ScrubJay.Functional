using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScrubJay.Functional.Json;

public sealed class OptionJsonConverter<T> : JsonConverter<Option<T>>
{
    public override Option<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return Option<T>.None;
        }

        var valueConverter = options.GetConverter<T>()!;
        var value = valueConverter.Read(ref reader, options);
        return Option<T>.Some(value!);
    }

    public override void Write(Utf8JsonWriter writer, Option<T> option, JsonSerializerOptions options)
    {
        if (option.IsSome(out var value))
        {
            var valueConverter = options.GetConverter<T>()!;
            valueConverter.Write(writer, value, options);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}