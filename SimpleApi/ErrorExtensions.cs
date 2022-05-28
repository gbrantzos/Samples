using Microsoft.AspNetCore.Mvc;

namespace SimpleApi;

public static class ErrorExtensions
{
    public static bool IsSystemError(this Error error) => error is SystemError;
    
    public static ActionResult ToActionResult(this Error error, string? instance)
    {
        var toReturn = error switch
        {
            ValidationError validationError => ToActionResult(validationError, instance),
            SystemError systemError => ToActionResult(systemError, instance),
            _ => throw new ArgumentException($"Unsupported error type: {error.GetType().Name}")
        };
        return toReturn;
    }
    
    

    private static ActionResult ToActionResult(ValidationError validationError, string? instance)
    {
        var details = new ProblemDetails
        {
            Title = validationError.Message,
            Status = StatusCodes.Status400BadRequest,
            Type = "https://httpstatuses.io/400",
            Detail = "Invalid request",
            Instance = instance,
            Extensions =
            {
                ["Errors"] = validationError.Errors.ToDictionary(v => v.Property, v => v.Message)
            }
        };
        return new BadRequestObjectResult(details);
    }

    private static ActionResult ToActionResult(SystemError systemError, string? instance)
    {
        var details = new ProblemDetails
        {
            Title = systemError.Message,
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://httpstatuses.io/500",
            Detail = "Unhandled exception",
            Instance = instance,
            Extensions =
            {
                ["Errors"] = systemError.AllMessages,
                ["Exception"] = systemError.Exception
            }
        };
        return new ObjectResult(details) {StatusCode = details.Status};
    }
}
