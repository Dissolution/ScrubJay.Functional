using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScrubJay.Functional.Json;

public sealed class ResultJsonConverter : JsonConverter<Result>
{
    public static ResultJsonConverter Default { get; } = new ();

    public override Result Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();
        
        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName)
            throw new JsonException();

        Result result;

        if (reader.ValueSpan.SequenceEqual("ok"u8) && reader.Read())
        {
            var unit = reader.GetString();
            if (unit != "()")
                throw new JsonException();
            result = Result.Ok;
        }
        else if (reader.ValueSpan.SequenceEqual("error"u8) && reader.Read())
        {
            var converter = options.GetConverter<Exception>()!;
            var ex = converter.Read(ref reader, options);
            result = Result.Error(ex!);
        }
        else
        {
            throw new JsonException();
        }

        if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
            throw new JsonException();

        return result;
    }
    
    public override void Write(Utf8JsonWriter writer, Result result, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        if (!result.IsError(out var error))
        {
            //ok
            writer.WritePropertyName("ok");
            writer.WriteStringValue("()");
        }
        else
        {
            writer.WritePropertyName("error");
            var converter = options.GetConverter<Exception>()!;
            converter.Write(writer, error, options);
        }
        
        writer.WriteEndObject();
    }
}