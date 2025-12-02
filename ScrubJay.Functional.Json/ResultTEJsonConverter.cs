using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScrubJay.Functional.Json;

public sealed class ResultJsonConverter<T, E> : JsonConverter<Result<T, E>>
{
    public override Result<T, E> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();
        reader.Read();

        if (reader.TokenType != JsonTokenType.PropertyName)
            throw new JsonException();

        Result<T, E> result;

        if (reader.ValueSpan.SequenceEqual("ok"u8) && reader.Read())
        {
            var converter = options.GetConverter<T>()!;
            var value = converter.Read(ref reader, options);
            result = Result<T, E>.Ok(value!);
        }
        else if (reader.ValueSpan.SequenceEqual("error"u8) && reader.Read())
        {
            var converter = options.GetConverter<E>()!;
            var error = converter.Read(ref reader, options);
            result = Result<T, E>.Error(error!);
        }
        else
        {
            throw new JsonException("Property was not named `ok` nor `error`");
        }

        if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
            throw new JsonException("Expected endobject");

        return result;
    }

    public override void Write(Utf8JsonWriter writer, Result<T, E> result, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (result.IsOk(out var ok, out var error))
        {
            writer.WritePropertyName("ok");
            var converter = options.GetConverter<T>()!;
            converter.Write(writer, ok, options);
        }
        else
        {
            writer.WritePropertyName("error");
            var converter = options.GetConverter<E>()!;
            converter.Write(writer, error, options);
        }

        writer.WriteEndObject();
    }
}