using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.OpenApi.Models;
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

        var html = 
            $"""
            <div><label>Environment</label>{environment.AspNetCoreEnvironment}<div/>
            <div><label>Version    </label>{environment.BuildInformation.Version}<div/>
            <div><label>Commit     </label>{environment.BuildInformation.Commit}<div/>
            <div><label>Build at   </label>{environment.BuildInformation.BuildAt} - <b>{environment.BuildInformation.Mode}</b><div/>
            """;
        
        services
            .AddEndpointsApiExplorer()
            .AddHttpContextAccessor()
            .AddProblemDetails(options => options.CustomizeProblemDetails = ErrorMapper.CustomizeProblemDetails)
            .AddSwaggerGen(options =>
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version     = "v1",
                    Title       = "Simple API",
                    Description = $"""
                                  Simple REST API sample over a CRUD application to \"gather\" best practices and techniques.
                                  <hr/>
                                  {html}
                                  """
                })
            );
        services.AddSingleton(environment);
        services.AddSingleton<RequestContextProvider>();
        services.AddSingleton<RequestContextEnricher>();
        services.AddTransient<ErrorMapper>();

        return services;
    }
}
