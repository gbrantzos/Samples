namespace Mediator.Commands
{
    public class ExecutionContext : IExecutionContext
    {
        public static ExecutionContext Default => new ExecutionContext();
    }
}
