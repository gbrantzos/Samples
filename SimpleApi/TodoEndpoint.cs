using Ardalis.ApiEndpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace SimpleApi;

public class ListTodoEndpoint : EndpointBaseAsync
    .WithRequest<int>
    .WithActionResult<QueryResult<TodoViewModel>>
{
    private readonly IMediator _mediator;

    public ListTodoEndpoint(IMediator mediator) => _mediator = mediator;

    [HttpGet(SimpleApiRoutes.Todo)]
    [SwaggerOperation(
        Summary = "TODOs List",
        Description = "List of TODO items",
        OperationId = "Todo.List",
        Tags = new[] {"TODOs Endpoint"})
    ]
    [Produces("application/json")]
    [ProducesResponseType(typeof(QueryResult<TodoViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<QueryResult<TodoViewModel>>> HandleAsync(
        [FromQuery] int accept,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await _mediator.Send(new SearchTodos(accept != 0), cancellationToken);
        if (result.HasErrors)
            return result.Error.ToActionResult(HttpContext.Request.Path.ToString());

        return result.Data;
    }
}

public class SaveTodoEndpoint : EndpointBaseAsync
    .WithRequest<TodoViewModel>
    .WithActionResult
{
    [HttpPost(SimpleApiRoutes.Todo)]
    [SwaggerOperation(
        Summary = "Save TODO",
        Description = "Create or update a TODO item",
        OperationId = "Todo.Save",
        Tags = new[] {"TODOs Endpoint"})
    ]
    [Consumes("application/json")]
    public override async Task<ActionResult> HandleAsync(TodoViewModel request, CancellationToken cancellationToken = new CancellationToken())
    {
        await Task.CompletedTask;
        return Ok("TODO saved");
    }
}
