namespace SimpleApi
{
    public abstract class Error
    {
        public string Message { get; set; }

        public Error(string message)
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
        public BusinessError(string message) : base(message)
        {
        }
    }

    public static class ErrorExtensions
    {
        public static bool IsSystemError(this Error error) => error is SystemError;
    }
}
