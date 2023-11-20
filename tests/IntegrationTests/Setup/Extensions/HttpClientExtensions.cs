using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SimpleAPI.IntegrationTests.Setup;

public static class HttpClientExtensions
{   
    public static Task<HttpResponseMessage> PostStringAsJsonAsync(this HttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)]
        string requestUri,
        string json)
        => client.PostAsync(requestUri, new StringContent(
            json,
            Encoding.UTF8,
            "application/json")
        );

    public static Task<HttpResponseMessage> PutStringAsJsonAsync(this HttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)]
        string requestUri,
        string json)
        => client.PutAsync(requestUri, new StringContent(
            json,
            Encoding.UTF8,
            "application/json")
        );
    
    public static async Task<T> GetFromApiAsync<T>(this HttpClient client, string location)
    {
        var response = await client.GetAsync(location);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return content.FromJson<T>();
    }

    public static async Task<int> CreateUsingApiAsync(this HttpClient client, string endpoint, string json)
    {
        var response = await client.PostStringAsJsonAsync(endpoint, json);
        var location = response.Headers.Location;

        return location.GetID();
    }
}
