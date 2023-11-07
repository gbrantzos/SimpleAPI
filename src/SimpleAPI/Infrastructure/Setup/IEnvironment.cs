using System.Reflection;

namespace SimpleAPI.Infrastructure;

public interface IEnvironment
{
    string Name { get; }
    string Description { get; }
    Assembly MainAssembly { get; }
    string BinaryPath { get; }
    string AsciiLogo { get; }
    BuildInformation BuildInformation { get; }
    string AspNetCoreEnvironment { get; }
}
