using Microsoft.AspNetCore.Diagnostics;

namespace SimpleAPI.Infrastructure.Setup;

public static class ProblemDetailsHelpers
{
    public static void CustomizeProblemDetails(ProblemDetailsContext context)
    {
        var requestContextProvider = context.HttpContext.RequestServices.GetRequiredService<RequestContextProvider>();
        var executionID = requestContextProvider.CurrentContext?.ExecutionID;
        if (executionID is not null)
        {
            context.ProblemDetails.Extensions.Add("executionID", executionID);
        }

        context.ProblemDetails.Type     = $"https://httpstatuses.io/{context.HttpContext.Response.StatusCode}";
        context.ProblemDetails.Instance = context.HttpContext.Request.Path;

        var hostingEnvironment = context.HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
        var exceptionHandlerFeature = context.HttpContext.Features.Get<IExceptionHandlerFeature>();
        if (exceptionHandlerFeature is not null)
        {
            var exception = exceptionHandlerFeature.Error;
            var exceptionDto = new ExceptionObject(exception);

            context.ProblemDetails.Detail = exceptionDto.Message;
            if (hostingEnvironment.IsDevelopment())
            {
                context.ProblemDetails.Extensions.Add("exception", exceptionDto);
            }
        }
    }
}
