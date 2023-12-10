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
            options.GetLevel = (context, elapsed, ex)
                => ex is OperationCanceledException
                    ? LogEventLevel.Verbose
                    : serilog(context, elapsed, ex);
        });
        builder.UseHttpMetrics();

        if (environment.IsDevelopment())
        {
            builder.UseSwagger();
            builder.UseSwaggerUI(options =>
            {
                options.HeadContent = """
                                      <style>
                                        .description {
                                            font-size: smaller;
                                        }
                                        .description label {
                                          width: 100px;
                                          display: inline-block;
                                          margin: 0 0 3px 0;
                                        }
                                      </style>
                                      """;
                options.DocumentTitle = "Simple API";
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Simple API - v1");
                options.DocExpansion(DocExpansion.List);
                options.DefaultModelsExpandDepth(-1);
            });
        }

        return builder;
    }

    private static IApplicationBuilder UseRequestContext(this IApplicationBuilder builder)
        => builder.UseMiddleware<RequestContextMiddleware>();
}
