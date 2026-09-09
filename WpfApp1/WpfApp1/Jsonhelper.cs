using System.Text.Json;
using System.Text.Json.Serialization;

public static class JsonHelper
{
    private static readonly JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static string SerializeObject(object obj)
    {
        return JsonSerializer.Serialize(obj, options);
    }

    public static T DeserializeObject<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, options);
    }
}
