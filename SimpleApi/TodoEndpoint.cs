using Ardalis.ApiEndpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace SimpleApi;

[Produces("application/json")]
public class TodoEndpoint : EndpointBaseAsync
    .WithoutRequest
    .WithActionResult<QueryResult<TodoViewModel>>
{
    private readonly IMediator _mediator;

    public TodoEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet(SimpleApiRoutes.Todo)]
    [SwaggerOperation(
        Summary = "TODOs List",
        Description = "List of TODO items",
        OperationId = "Todo.List",
        Tags = new[] { "TODOs Endpoint" })
    ]
    public override async Task<ActionResult<QueryResult<TodoViewModel>>> HandleAsync(
        CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await _mediator.Send(new SearchTodos(), cancellationToken);
        if (result.HasErrors)
            return BadRequest(result.ErrorOrDefault());

        var list = result.DataOrDefault();
        if (list is null)
            return StatusCode(500);
        
        return list;
    }
}
