using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SimpleAPI.Core.Base;
using SimpleAPI.Web.Setup.Context;

namespace SimpleAPI.Web.ErrorMapping;

public class ErrorMapper
{
    public const string ExecutionID = "executionID";
    private readonly RequestContextProvider _contextProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ErrorMapper(RequestContextProvider contextProvider, IHttpContextAccessor httpContextAccessor)
    {
        _contextProvider     = contextProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    public ProblemDetails MapToProblemDetails(Error error)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        var instance = $"{request?.Method} {request?.Path}";

        var toReturn = new ProblemDetails()
        {
            Type     = TypeForStatusCode(StatusCodeForErrorKind(error.Kind)),
            Title    = TitleForErrorKind(error.Kind),
            Status   = (int)StatusCodeForErrorKind(error.Kind),
            Detail   = error.Message,
            Instance = instance
        };
        toReturn.Extensions.Add(ExecutionID, _contextProvider.CurrentContext?.ExecutionID);

        return toReturn;
    }

    private static string TypeForStatusCode(HttpStatusCode statusCode)
        => $"https://httpstatuses.io/{(int)statusCode}";

    private static string TitleForErrorKind(ErrorKind errorKind)
    {
        return errorKind switch
        {
            ErrorKind.Generic => "Generic error",
            ErrorKind.NotFound => "Not found",
            ErrorKind.ValidationFailed => "Bad request",
            ErrorKind.ModifiedEntry => "Entry modified",
            _ => throw new ArgumentOutOfRangeException(nameof(errorKind), errorKind, null)
        };
    }

    private static HttpStatusCode StatusCodeForErrorKind(ErrorKind errorKind)
    {
        return errorKind switch
        {
            ErrorKind.Generic => HttpStatusCode.InternalServerError,
            ErrorKind.NotFound => HttpStatusCode.NotFound,
            ErrorKind.ValidationFailed => HttpStatusCode.BadRequest,
            ErrorKind.ModifiedEntry => HttpStatusCode.Conflict,
            _ => throw new ArgumentOutOfRangeException(nameof(errorKind), errorKind, null)
        };
    }
    
    public static void CustomizeProblemDetails(ProblemDetailsContext context)
    {
        var requestContextProvider = context.HttpContext.RequestServices.GetRequiredService<RequestContextProvider>();
        var executionID = requestContextProvider.CurrentContext?.ExecutionID;
        if (executionID is not null)
        {
            context.ProblemDetails.Extensions.Add(ExecutionID, executionID);
        }

        context.ProblemDetails.Type     = TypeForStatusCode((HttpStatusCode)context.HttpContext.Response.StatusCode);
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
