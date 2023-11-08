using Prometheus;
using Serilog;
using SimpleAPI.Web.HostSetup.Context;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace SimpleAPI.Web.HostSetup;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder ConfigurePipeline(this IApplicationBuilder builder, IWebHostEnvironment environment)
    {
        // Adding Request Context middleware first so that we can have executionID created as soon as possible
        builder.UseRequestContext();

        builder.UseExceptionHandler();
        builder.UseStatusCodePages();
        builder.UseSerilogRequestLogging();
        builder.UseHttpMetrics();

        if (environment.IsDevelopment())
        {
            builder.UseSwagger();
            builder.UseSwaggerUI(options =>
            {
                options.DocExpansion(DocExpansion.List);
                options.DefaultModelsExpandDepth(-1);
            });
        }

        return builder;
    }

    private static IApplicationBuilder UseRequestContext(this IApplicationBuilder builder)
        => builder.UseMiddleware<RequestContextMiddleware>();
}

