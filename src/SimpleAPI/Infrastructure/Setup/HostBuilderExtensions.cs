using Autofac.Extensions.DependencyInjection;
using Serilog;
using SimpleAPI.Infrastructure.Setup;

namespace SimpleAPI.Infrastructure;

public static class HostBuilderExtensions
{
    public static IHostBuilder PrepareHost(this IHostBuilder builder, IConfiguration configuration)
    {
        SerilogHelpers.SetLoggingPath();
        
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
}
