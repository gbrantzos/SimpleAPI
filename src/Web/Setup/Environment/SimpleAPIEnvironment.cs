using System.Globalization;
using System.Reflection;
using System.Text;

namespace SimpleAPI.Web.Setup.Environment;

public class SimpleAPIEnvironment : IEnvironment
{
    public string Name => "Simple API";
    public string Description => "Simple API by GbWorks";
    public Assembly MainAssembly { get; }
    public string BinaryPath { get; }
    public BuildInformation BuildInformation => BuildInformation.Instance;
    public string AspNetCoreEnvironment { get; }


    public string AsciiLogo => @"
  ______  __                       __                ______  _______  ______ 
 /      \|  \                     |  \              /      \|       \|      \
|  ▓▓▓▓▓▓\\▓▓______ ____   ______ | ▓▓ ______      |  ▓▓▓▓▓▓\ ▓▓▓▓▓▓▓\\▓▓▓▓▓▓
| ▓▓___\▓▓  \      \    \ /      \| ▓▓/      \     | ▓▓__| ▓▓ ▓▓__/ ▓▓ | ▓▓  
 \▓▓    \| ▓▓ ▓▓▓▓▓▓\▓▓▓▓\  ▓▓▓▓▓▓\ ▓▓  ▓▓▓▓▓▓\    | ▓▓    ▓▓ ▓▓    ▓▓ | ▓▓  
 _\▓▓▓▓▓▓\ ▓▓ ▓▓ | ▓▓ | ▓▓ ▓▓  | ▓▓ ▓▓ ▓▓    ▓▓    | ▓▓▓▓▓▓▓▓ ▓▓▓▓▓▓▓  | ▓▓  
|  \__| ▓▓ ▓▓ ▓▓ | ▓▓ | ▓▓ ▓▓__/ ▓▓ ▓▓ ▓▓▓▓▓▓▓▓    | ▓▓  | ▓▓ ▓▓      _| ▓▓_ 
 \▓▓    ▓▓ ▓▓ ▓▓ | ▓▓ | ▓▓ ▓▓    ▓▓ ▓▓\▓▓     \    | ▓▓  | ▓▓ ▓▓     |   ▓▓ \
  \▓▓▓▓▓▓ \▓▓\▓▓  \▓▓  \▓▓ ▓▓▓▓▓▓▓ \▓▓ \▓▓▓▓▓▓▓     \▓▓   \▓▓\▓▓      \▓▓▓▓▓▓
                         | ▓▓                                                
                         | ▓▓                                                
                          \▓▓                                                
";

    private SimpleAPIEnvironment()
    {
        MainAssembly = typeof(SimpleAPIEnvironment).Assembly;
        BinaryPath   = Path.GetDirectoryName(MainAssembly.Location) ?? String.Empty;
        AspNetCoreEnvironment =
            System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToUpper(CultureInfo.InvariantCulture) ??
            "PRODUCTION";
    }

    public static IEnvironment Current() => new SimpleAPIEnvironment();
}

public static class BackendEnvironmentExtensions
{
    public static string GetBackendRelativePath(this IEnvironment environment, string path)
        => Path.Combine(environment.BinaryPath, path);

    public static string GetLogo(this IEnvironment environment)
    {
        var sb = new StringBuilder();
        sb.AppendLine(environment.AsciiLogo);
        sb.AppendLine(CultureInfo.InvariantCulture, $"Environment:    {environment.AspNetCoreEnvironment}");
        sb.AppendLine(environment.BuildInformation.ToDisplayString());
        
        return sb.ToString();
    }
    public static void DisplayLogo(this IEnvironment environment)
    {
        Console.WriteLine(environment.GetLogo());
        Console.WriteLine();
    }
}
