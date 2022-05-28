namespace SimpleApi
{
    public abstract class Error
    {
        public string Message { get; set; }

        protected Error(string message)
        {
            Message = message;
        }
    }

    public class SystemError : Error
    {
        public Exception Exception { get; }
        public IReadOnlyList<string> AllMessages { get; }

        public SystemError(Exception exception) : base(exception.Message)
        {
            Exception = exception.ThrowIfNull();
            AllMessages = GetInnerMessages(exception);
        }

        private static List<string> GetInnerMessages(Exception exception)
        {
            var messages = new List<string>();
            var inner = exception;
            do
            {
                if (!messages.Contains(inner.Message))
                    messages.Add(inner.Message);
                inner = inner.InnerException;
            }
            while (inner != null);

            return messages;
        }
    }

    public abstract class BusinessError : Error
    {
        protected BusinessError(string message) : base(message)
        {
        }
    }

    public class ValidationError : BusinessError
    {
        public record ValidationFail(string Property, string Message);

        private readonly IEnumerable<ValidationFail> _errors;
        public IReadOnlyList<ValidationFail> Errors => _errors.ToList().AsReadOnly();

        public ValidationError(IEnumerable<ValidationFail> errors) : base(nameof(ValidationError))
        {
            _errors = errors;
        }
    }
}
