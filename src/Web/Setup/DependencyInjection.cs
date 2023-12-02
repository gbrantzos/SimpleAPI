using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.AspNetCore.Http.Json;
using SimpleAPI.Web.ErrorMapping;
using SimpleAPI.Web.Setup.Context;
using SimpleAPI.Web.Setup.Environment;

namespace SimpleAPI.Web.Setup;

public static class DependencyInjection
{
    public static IServiceCollection AddHostServices(this IServiceCollection services, IEnvironment environment)
    {
        // Force returning BadRequest instead of throwing
        services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = false);
        services.Configure<RouteOptions>(options => options.LowercaseUrls            = true);
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Encoder              = JavaScriptEncoder.Create(UnicodeRanges.All);
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.DictionaryKeyPolicy  = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services
            .AddEndpointsApiExplorer()
            .AddHttpContextAccessor()
            .AddProblemDetails(options => options.CustomizeProblemDetails = ErrorMapper.CustomizeProblemDetails)
            .AddSwaggerGen();

        services.AddSingleton(environment);
        services.AddSingleton<RequestContextProvider>();
        services.AddSingleton<RequestContextEnricher>();
        services.AddTransient<ErrorMapper>();

        return services;
    }
}
