using Arian.Quantiq.Domain.Common.Results;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace Arian.Quantiq.Middlewares;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception, "Exception occurred: {Message}", exception.Message);

        ApplicationResult<AppVoid> problemDetails = (
                new ErrorContainer(["Server Error."]),
                HttpStatusCode.InternalServerError);

        httpContext.Response.StatusCode = (int)problemDetails.HttpStatusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}