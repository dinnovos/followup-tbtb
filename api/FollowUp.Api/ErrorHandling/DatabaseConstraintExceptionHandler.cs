using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FollowUp.Api.ErrorHandling;

public class DatabaseConstraintExceptionHandler : IExceptionHandler
{
    private const int ForeignKeyViolation = 547;
    private const int UniqueConstraintViolation = 2627;
    private const int UniqueIndexViolation = 2601;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DbUpdateException { InnerException: SqlException sqlException })
        {
            return false;
        }

        ProblemDetails? problem = sqlException.Number switch
        {
            ForeignKeyViolation => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid reference",
                Detail = "One of the referenced records (e.g. the manager) does not exist."
            },
            UniqueConstraintViolation or UniqueIndexViolation => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = "This record already exists."
            },
            _ => null
        };

        if (problem is null)
        {
            return false;
        }

        httpContext.Response.StatusCode = problem.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
