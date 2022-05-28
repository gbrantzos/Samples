namespace SimpleApi
{
    public class Result<TData, TError>
    {
        private readonly TData? _data;
        private readonly TError? _error;

        public bool HasErrors { get; }

        private Result(TData? data, TError? error = default, bool hasErrors = false)
        {
            // Although TData is nullable, this constructor should not allow null values!
            if (!hasErrors)
                ArgumentNullException.ThrowIfNull(data);
            
            _data = data;
            _error = error;
            HasErrors = hasErrors;
        }

        private Result(TError error) : this(default, error, true)
        {
            ArgumentNullException.ThrowIfNull(error);
        }

        public static implicit operator Result<TData, TError>(TData data) => new(data);
        public static implicit operator Result<TData, TError>(TError error) => new(error);

        public T Match<T>(Func<TData, T> dataFunc, Func<TError, T> errorFunc)
        {
            ArgumentNullException.ThrowIfNull(dataFunc);
            ArgumentNullException.ThrowIfNull(errorFunc);

            return HasErrors
                ? errorFunc(_error ?? throw new ArgumentException(nameof(_error)))
                : dataFunc(_data ?? throw new ArgumentException(nameof(_data)));
        }

        public TData Data => _data ?? throw new ArgumentNullException(nameof(_data));
        public TError Error => _error ?? throw new ArgumentNullException(nameof(_error));
    }
}
