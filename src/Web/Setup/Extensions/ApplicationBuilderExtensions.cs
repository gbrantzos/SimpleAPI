using Prometheus;
using Serilog;
using Serilog.Events;
using SimpleAPI.Web.Setup.Context;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace SimpleAPI.Web.Setup;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder ConfigurePipeline(this IApplicationBuilder builder, IWebHostEnvironment environment)
    {
        // Adding Request Context middleware first so that we can have executionID created as soon as possible
        builder.UseRequestContext();

        builder.UseExceptionHandler();
        builder.UseStatusCodePages();
        builder.UseSerilogRequestLogging(options =>
        {
            var serilog = options.GetLevel;
            options.GetLevel = (context, elapsed, ex) =>
            {
                if (ex is OperationCanceledException)
                    return LogEventLevel.Verbose;
                    
                return serilog(context, elapsed, ex);
            };
        });
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

