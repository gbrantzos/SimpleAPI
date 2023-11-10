using System.Globalization;
using System.Reflection;

namespace SimpleAPI.Web.Setup;

public class BuildInformation
{
    public string Version { get; private init; } = "0.1.0";
    public string[] Tags { get; private init; } = Array.Empty<string>();
    public DateTime BuildAt { get; private init; } = DateTime.MinValue;
    public string Commit { get; private init; } = String.Empty;
    public bool IsDirty { get; private init; }
    public bool IsDebug { get; private init; } = true;
    public string Mode => IsDebug ? "DEBUG" : "RELEASE";

    public static BuildInformation Instance { get; }

    static BuildInformation()
    {
        var type = typeof(BuildInformation);
        var assembly = type.Assembly;
        var assemblyVersion = assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion ??
            "0.1.0-DEV";

        using var buildData = assembly.GetManifestResourceStream($"{type.FullName}.txt");
        if (buildData == null)
            throw new ArgumentException("Build information stream is null!");

        using var reader = new StreamReader(buildData);
        var text = reader.ReadToEnd().Trim();
        var infoParts = text.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
        if (infoParts!.Length < 3)
            throw new ArgumentException("Invalid build information.");

        var tags = infoParts[2][(infoParts[2].IndexOf(":", StringComparison.CurrentCultureIgnoreCase) + 1)..]
            .Trim()
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var version = tags.FirstOrDefault(t => t.StartsWith("v", StringComparison.CurrentCultureIgnoreCase)) ??
            $"v{assemblyVersion}";

        Instance = new BuildInformation
        {
            Version = version,
            BuildAt = DateTime.ParseExact(infoParts[0], "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture),
            Commit  = infoParts[1][(infoParts[1].IndexOf(":", StringComparison.CurrentCultureIgnoreCase) + 1)..].Trim(),
            IsDirty = "1" == infoParts[3][(infoParts[3].IndexOf(":", StringComparison.CurrentCultureIgnoreCase) + 1)..].Trim(),
            Tags    = tags.Where(t => !t.Equals(version, StringComparison.OrdinalIgnoreCase)).ToArray(),
#if DEBUG
            IsDebug = true
#else
            IsDebug = false
#endif
        };
    }

    public string ToDisplayString()
    {
        var isDirty = IsDirty ? " [DIRTY repo]" : String.Empty;
        var tags = Tags.Length == 0 ? "No tags" : String.Join(", ", Tags);
        return
            $"Version: \t{Version}\nCommit:\t\t{Commit}{isDirty}\nTags:\t\t{tags}\nBuild at:\t{BuildAt:yyyy/MM/dd HH:mm:ss} - [{Mode} build]";
    }
}
