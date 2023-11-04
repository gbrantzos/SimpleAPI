using Serilog;
using SimpleAPI.Infrastructure.Setup;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace SimpleAPI.Infrastructure;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder ConfigurePipeline(this IApplicationBuilder builder, IWebHostEnvironment environment)
    {
        // Adding Request Context middleware first so that we can have executionID created as soon as possible
        builder.UseRequestContext();

        builder.UseExceptionHandler();
        builder.UseStatusCodePages();
        builder.UseSerilogRequestLogging();

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

