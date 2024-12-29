using System.Diagnostics.CodeAnalysis;

namespace MinimalAPI;

public class Error
{
}

public class Result
{
    private readonly Error? _error;
    public Error Error => IsFailure ? _error! : throw new InvalidOperationException("Result does not have errors.");

    public bool IsSuccess => _error is null;
    public bool IsFailure => !IsSuccess;

    protected Result(Error? error) => _error = error;

    public static Result Failure(Error error) => new Result(error);
    public static Result Success() => new Result(null);
}

public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;
    public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("Result has errors.");

    private Result(TValue? data, Error? error = default, bool hasErrors = false) : base(error)
    {
        if (hasErrors)
        {
            // When we do have an Error, make sure it is not be null
            ArgumentNullException.ThrowIfNull(error);
        }
        else
        {
            // When we don't have an Error, we should have value
            ArgumentNullException.ThrowIfNull(data);
        }

        _value = data;
    }

    public static Result<TValue> Success(TValue value) => new(value);

    public static implicit operator Result<TValue>(TValue value) => new(value);
    public static implicit operator Result<TValue>(Error error) => new(default, error, true);
}