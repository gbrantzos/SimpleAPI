using System.Text.Json;

namespace SimpleAPI.Core;

public static class LoggingExtensions
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
    };

    public static string ToJsonForLogging(this object obj, bool borders = true, JsonSerializerOptions? options = null)
    {
        var json = JsonSerializer.Serialize(obj, options ?? SerializerOptions);
        if (borders)
        {
            var objType = obj.GetType().Name;
            var borderPart = new String('=', (80 - 6 - objType.Length) / 2);
            var topBorder = $"{borderPart}>  {objType}  <{borderPart}";
            var bottomBorder = new String('-', topBorder.Length);
            return $"{topBorder}\n{json}\n{bottomBorder}";
        }
        return json;
    }
}
