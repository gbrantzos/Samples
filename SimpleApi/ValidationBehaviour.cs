using FluentValidation;
using MediatR;

namespace SimpleApi;

public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse, Error>>
    where TRequest : Request<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehaviour<TRequest, TResponse>> _logger;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehaviour<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public Task<Result<TResponse, Error>> Handle(TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<Result<TResponse, Error>> next)
    {
        if (!_validators.Any())
        {
            return next();
        }

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(fail => fail != null)
            .ToList();

        if (failures.Any())
        {
            _logger.LogWarning("Validation for request {RequestInstance} failed!", request);
            var errors = failures
                .Select(f => new ValidationError.ValidationFail(f.PropertyName, f.ErrorMessage));
            var toReturn = new ValidationError(errors);
            return Task.FromResult<Result<TResponse, Error>>(toReturn);
        }

        return next();
    }
}
