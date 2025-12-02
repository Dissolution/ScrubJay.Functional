using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScrubJay.Functional.Json;

public sealed class ResultJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (typeToConvert == typeof(Result))
            return true;
        if (!typeToConvert.IsGenericType)
            return false;
        var genericType = typeToConvert.GetGenericTypeDefinition();
        return genericType == typeof(Result<>) || genericType == typeof(Result<,>);
    }
    
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert == typeof(Result))
        {
            return ResultJsonConverter.Default;
        }

        if (!typeToConvert.IsGenericType)
            throw new JsonException();
        
        var genericTypeDef = typeToConvert.GetGenericTypeDefinition();
        var genericTypes =  typeToConvert.GetGenericArguments();
        
        if (genericTypeDef == typeof(Result<>))
        {
            object inst = Activator.CreateInstance(typeof(ResultJsonConverter<>)!.MakeGenericType(genericTypes))!;
            if (inst is JsonConverter converter)
                return converter;
            throw new JsonException();
        }
        
        if (genericTypeDef == typeof(Result<,>))
        {
            object inst = Activator.CreateInstance(typeof(ResultJsonConverter<,>)!.MakeGenericType(genericTypes))!;
            if (inst is JsonConverter converter)
                return converter;
            throw new JsonException();
        }
        
        throw new JsonException();
    }
}