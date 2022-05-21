namespace SimpleApi
{
    public class SearchTodos : Query<QueryResult<TodoViewModel>>
    {
        public override string ToString()
        {
            return "Search TODOs";
        }
    }

    public class TodoViewModel
    {
        public string Description { get; init; } = String.Empty;
        public bool IsDone { get; init; }
    }

    public class SearchTodosHandler : Handler<SearchTodos, QueryResult<TodoViewModel>>
    {
        private readonly ILogger<SearchTodosHandler> _logger;

        public SearchTodosHandler(ILogger<SearchTodosHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<Result<QueryResult<TodoViewModel>, Error>> HandleCore(SearchTodos request,
            CancellationToken cancellationToken)
        {
            _logger.LogDebug("Handling command '{Command}'", request.ToString());
            await Task.CompletedTask;
            var list = new List<TodoViewModel>
            {
                new TodoViewModel
                {
                    Description = "Build a cool API",
                    IsDone = false
                }
            };
            
            return new QueryResult<TodoViewModel>(list);
        }
    }
}
