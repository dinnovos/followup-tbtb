using FollowUp.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FollowUp.Api.ErrorHandling;

public class DuplicatePatientExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DuplicatePatientException duplicateException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Duplicate patient",
            Detail = duplicateException.Message
        }, cancellationToken);

        return true;
    }
}
