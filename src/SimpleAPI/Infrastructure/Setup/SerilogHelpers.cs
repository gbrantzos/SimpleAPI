namespace SimpleAPI.Infrastructure.Setup;

public static class SerilogHelpers
{
    public static void SetLoggingPath(string path = "logs")
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var logPath = Path.Combine(basePath, path);

        Environment.SetEnvironmentVariable("LOG_PATH", logPath);
    }
}
