using Autofac.Extensions.DependencyInjection;
using Serilog;
using SimpleAPI.Web.Setup.Context;

namespace SimpleAPI.Web.Setup;

public static class HostBuilderExtensions
{
    public static IHostBuilder PrepareHost(this IHostBuilder builder, IConfiguration configuration)
    {
        SetLoggingPath();
        
        return builder
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .UseSerilog((_, services, loggerConfig) =>
            {
                var requestContextEnricher = services.GetRequiredService<RequestContextEnricher>();
                loggerConfig
                    .ReadFrom.Configuration(configuration)
                    .ReadFrom.Services(services)
                    .Enrich.With(requestContextEnricher);
            });
    }

    private static void SetLoggingPath(string path = "logs")
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var logPath = Path.Combine(basePath, path);

        System.Environment.SetEnvironmentVariable("LOG_PATH", logPath);
    }
}
