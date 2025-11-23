using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScrubJay.Functional.Json;

public class OptionJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType &&
            typeToConvert.GetGenericTypeDefinition() == typeof(Option<>);
    }
    
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converter = Activator.CreateInstance(typeof(OptionJsonConverter<>).MakeGenericType(typeToConvert)) as JsonConverter;
        ArgumentNullException.ThrowIfNull(converter);
        return converter;
    }
}

public sealed class OptionJsonConverter<T> : JsonConverter<Option<T>>
{
    public override Option<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return Option<T>.None;
        }

        var valueConverter = options.GetConverter(typeof(T)) as JsonConverter<T>;
        ArgumentNullException.ThrowIfNull(valueConverter);
        var value = valueConverter.Read(ref reader, typeof(T), options);
        return Option<T>.Some(value!);
    }
    
    public override void Write(Utf8JsonWriter writer, Option<T> option, JsonSerializerOptions options)
    {
        if (option.IsSome(out var value))
        {
            var valueConverter = options.GetConverter(typeof(T)) as JsonConverter<T>;
            ArgumentNullException.ThrowIfNull(valueConverter);
            valueConverter.Write(writer, value, options);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}