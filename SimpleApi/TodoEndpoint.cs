using Ardalis.ApiEndpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace SimpleApi;

[Produces("application/json")]
public class TodoEndpoint : EndpointBaseAsync
    .WithRequest<int>
    .WithActionResult<QueryResult<TodoViewModel>>
{
    private readonly IMediator _mediator;

    public TodoEndpoint(IMediator mediator) => _mediator = mediator;

    [HttpGet(SimpleApiRoutes.Todo)]
    [SwaggerOperation(
        Summary = "TODOs List",
        Description = "List of TODO items",
        OperationId = "Todo.List",
        Tags = new[] {"TODOs Endpoint"})
    ]
    [ProducesResponseType(typeof(QueryResult<TodoViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<QueryResult<TodoViewModel>>> HandleAsync(
        //[FromQuery] bool acceptTerms,
        [FromRoute] int id,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await _mediator.Send(new SearchTodos(id == 1), cancellationToken);
        if (result.HasErrors)
            return result.Error.ToActionResult(HttpContext.Request.Path.ToString());

        return result.Data;
    }
}
