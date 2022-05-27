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

        public SystemError(Exception exception) : base(exception.Message)
        {
            Exception = exception;
        }
    }

    public class BusinessError : Error
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

    public static class ErrorExtensions
    {
        public static bool IsSystemError(this Error error) => error is SystemError;
    }
}
