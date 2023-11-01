using Autofac.Extensions.DependencyInjection;
using Serilog;

namespace SimpleAPI.Infrastructure.Setup;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddAutofac(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseServiceProviderFactory(new AutofacServiceProviderFactory());

        return hostBuilder;
    }

    public static IHostBuilder AddSerilog(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        hostBuilder.UseSerilog((_, services, loggerConfig) =>
        {
            var requestContextEnricher = services.GetRequiredService<RequestContextEnricher>();
            loggerConfig
                .ReadFrom.Configuration(configuration)
                .ReadFrom.Services(services)
                .Enrich.With(requestContextEnricher);
        });

        return hostBuilder;
    }
}
