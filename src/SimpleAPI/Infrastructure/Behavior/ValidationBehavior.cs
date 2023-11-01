using FluentValidation;
using MediatR;
using SimpleAPI.Application.Core;

namespace SimpleAPI.Infrastructure.Behavior;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse, Error>>
    where TRequest : Request<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;

    public async Task<Result<TResponse, Error>> Handle(TRequest request,
        RequestHandlerDelegate<Result<TResponse, Error>> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();
        if (failures.Count == 0)
            return await next();

        var errorDetails = failures.Select(r => new
            {
                Property = r.PropertyName,
                Message  = r.ErrorMessage
            })
            .GroupBy(r => r.Property)
            .ToDictionary(
                g => g.Key,
                g => (object?)(g.Select(r => r.Message).ToArray()));
        return Error.Create(ErrorKind.ValidationFailed, "Invalid request", errorDetails);
    }
}
