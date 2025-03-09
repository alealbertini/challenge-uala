using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

public sealed class ExceptionHandler(ILogger<ExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<ExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var result = new ProblemDetails();
        switch (exception)
        {
            case InvalidDataException invalidDataException:
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                result = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Type = invalidDataException.GetType().Name,
                    Title = "Invalid parameters",
                    Detail = invalidDataException.Message,
                    Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
                };
                _logger.LogError(invalidDataException, $"Exception occured : {invalidDataException.Message}");
                break;
            default:
                result = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Type = exception.GetType().Name,
                    Title = "An unexpected error occurred",
                    Detail = exception.Message,
                    Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
                };
                _logger.LogError(exception, $"Exception occured : {exception.Message}");
                break;
        }
        await httpContext.Response.WriteAsJsonAsync(result, cancellationToken: cancellationToken);
        return true;
    }
}