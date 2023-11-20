namespace SimpleAPI.IntegrationTests.Setup;

public static class HttpClientExtensions
{
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
