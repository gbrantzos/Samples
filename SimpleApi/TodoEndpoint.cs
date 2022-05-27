﻿using Ardalis.ApiEndpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace SimpleApi;

[Produces("application/json")]
public class TodoEndpoint : EndpointBaseAsync
    .WithRequest<bool>
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
    public override async Task<ActionResult<QueryResult<TodoViewModel>>> HandleAsync(
        [FromQuery] bool acceptTerms,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await _mediator.Send(new SearchTodos() { TermsAccepted = acceptTerms} , cancellationToken);
        if (result.HasErrors)
        {
            var error = result.ErrorOrDefault();
            if (error is ValidationError validationError)
                return BadRequest(validationError);
            return BadRequest(result.ErrorOrDefault()?.Message ?? "Error!");
        }

        var list = result.DataOrDefault();
        if (list is null)
            return StatusCode(500);

        return list;
    }
}
