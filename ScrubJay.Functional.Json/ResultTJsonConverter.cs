using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScrubJay.Functional.Json;

public class ResultTJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType &&
            typeToConvert.GetGenericTypeDefinition() == typeof(Result<>);
    }
    
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converter = Activator.CreateInstance(typeof(ResultJsonConverter<>).MakeGenericType(typeToConvert)) as JsonConverter;
        ArgumentNullException.ThrowIfNull(converter);
        return converter;
    }
}

public sealed class ResultJsonConverter<T> : JsonConverter<Result<T>>
{
    public override Result<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();
        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName)
            throw new JsonException();

        Result<T> result;

        if (reader.ValueSpan.SequenceEqual("ok"u8) && reader.Read())
        {
            var converter = options.GetConverter(typeof(T)) as JsonConverter<T>;
            var value = converter.Read(ref reader, typeof(T), options);
            result = value;
        }
        else if (reader.ValueSpan.SequenceEqual("error"u8) && reader.Read())
        {
            var converter = options.GetConverter(typeof(Exception)) as JsonConverter<Exception>;
            var ex = converter.Read(ref reader, typeof(T), options);
            result = ex;
        }
        else
        {
            throw new JsonException();
        }

        if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
            throw new JsonException();

        return result;
    }
    
    public override void Write(Utf8JsonWriter writer, Result<T> result, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        if (result.IsOk(out var ok, out var error))
        {
            writer.WritePropertyName("ok");
            var converter = options.GetConverter(typeof(T)) as JsonConverter<T>;
            converter.Write(writer, ok, options);
        }
        else
        {
            writer.WritePropertyName("error");
            var converter = options.GetConverter(typeof(Exception)) as JsonConverter<Exception>;
            converter.Write(writer, error, options);
        }
        
        writer.WriteEndObject();
    }
}