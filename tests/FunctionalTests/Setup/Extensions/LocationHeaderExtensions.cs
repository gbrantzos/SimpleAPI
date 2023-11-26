namespace SimpleAPI.FunctionalTests.Setup;

public static class LocationHeaderExtensions
{
    public static int GetID(this Uri? location)
    {
        var id = location?
                .ToString()
                .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .LastOrDefault() ??
            throw new InvalidOperationException($"Could not get ID from location {location}");

        return Convert.ToInt32(id);
    }
}
