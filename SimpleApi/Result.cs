namespace SimpleApi
{
    public class Result<TData, TError> where TError : Error
    {
        private readonly TData? _data;
        private readonly TError? _error;
        private readonly bool _hasErrors;

        public bool HasErrors => _hasErrors;

        private Result(TData? data, TError? error = default, bool hasErrors = false)
        {
            _data = data;
            _error = error;
            _hasErrors = hasErrors;
        }

        private Result(TError error) : this(default, error, true)
        {
        }

        public static implicit operator Result<TData, TError>(TData data) => new(data);
        public static implicit operator Result<TData, TError>(TError error) => new(error);

        public T Match<T>(Func<TData, T> dataFunc, Func<TError, T> errorFunc)
        {
            ArgumentNullException.ThrowIfNull(dataFunc);
            ArgumentNullException.ThrowIfNull(errorFunc);

            return _hasErrors
                ? errorFunc(_error ?? throw new ArgumentException(nameof(_error)))
                : dataFunc(_data ?? throw new ArgumentException(nameof(_data)));
        }
        
        public TData? DataOrDefault() => this.Match(d => d, r => default(TData));
        public TError? ErrorOrDefault() => this.Match(d => default(TError), r => r);
    }
}
