using System.Text.Json;

namespace SimpleAPI.Web.IntegrationTests.Setup;

public static class SerializationExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static string ToJson(this object @object, JsonSerializerOptions? serializerOptions = null)
        => JsonSerializer.Serialize(@object, serializerOptions ?? JsonSerializerOptions);

    public static T FromJson<T>(this string str, JsonSerializerOptions? serializerOptions = null)
        => JsonSerializer.Deserialize<T>(str, serializerOptions ?? JsonSerializerOptions) ??
            throw new InvalidOperationException($"Could not serialize to {typeof(T).Name}: {str}");
}
