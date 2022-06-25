using FluentValidation;
using MediatR;

namespace SimpleApi;

public record SearchTodos(bool TermsAccepted) : Query<QueryResult<TodoViewModel>>;

public record TodoViewModel(string Description, bool IsDone);

public class SearchTodosValidator : AbstractValidator<SearchTodos>
{
    public SearchTodosValidator()
    {
        RuleFor(x => x.TermsAccepted)
            .Equal(true)
            .WithMessage("You must accept terms");
    }
}

public class SearchTodosHandler : Handler<SearchTodos, QueryResult<TodoViewModel>>
{
    private readonly ILogger<SearchTodosHandler> _logger;
    private readonly IMediator _mediator;
    private readonly DummyService _service;

    public SearchTodosHandler(ILogger<SearchTodosHandler> logger, IMediator mediator, DummyService service)
    {
        _logger = logger;
        _mediator = mediator;
        _service = service;
    }

    protected override async Task<Result<QueryResult<TodoViewModel>, Error>> HandleCore(SearchTodos request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling command '{Command}'", request.ToString());
        await Task.CompletedTask;
        var list = new List<TodoViewModel>
        {
            new("Build a cool API", false)
        };

        // throw new NotImplementedException();
        // _service.WhoAmI();
        var task = _mediator.Publish(new Ping(), cancellationToken);
        _ = task.ContinueWith(_ =>
                _logger.LogError(task.Exception,
                    "Error: {Message}",
                    task.Exception?.Message ?? "Task did not fail!"),
            cancellationToken);
        return new QueryResult<TodoViewModel>(list);
    }
}
