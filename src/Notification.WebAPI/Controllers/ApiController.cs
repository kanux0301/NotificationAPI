using Microsoft.AspNetCore.Mvc;
using Notification.Application.Common;

namespace Notification.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiController : ControllerBase
{
    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok();
        }

        return HandleError(result.Error);
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return HandleError(result.Error);
    }

    protected IActionResult HandleCreatedResult<T>(Result<T> result, string routeName, object? routeValues = null)
    {
        if (result.IsSuccess)
        {
            return CreatedAtRoute(routeName, routeValues ?? new { id = result.Value }, result.Value);
        }

        return HandleError(result.Error);
    }

    private IActionResult HandleError(Error error)
    {
        return error.Code switch
        {
            var code when code.StartsWith("Validation.") => BadRequest(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = error.Message,
                Status = StatusCodes.Status400BadRequest
            }),
            var code when code.EndsWith(".NotFound") => NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = error.Message,
                Status = StatusCodes.Status404NotFound
            }),
            var code when code.StartsWith("Conflict.") => Conflict(new ProblemDetails
            {
                Title = "Conflict",
                Detail = error.Message,
                Status = StatusCodes.Status409Conflict
            }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Server Error",
                Detail = error.Message,
                Status = StatusCodes.Status500InternalServerError
            })
        };
    }
}
