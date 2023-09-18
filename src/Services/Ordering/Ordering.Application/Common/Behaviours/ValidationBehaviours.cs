using FluentValidation;
using MediatR;
using ValidationException = Infrastructure.Exceptions.ValidationException;

namespace Ordering.Application.Common.Behaviours;

public class ValidationBehaviours<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviours(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults =
            await Task.WhenAll(_validators.Select(v => v.ValidateAsync(request, cancellationToken)));

        var failures = validationResults.Where(result => result.Errors.Any())
            .SelectMany(result => result.Errors)
            .ToList();

        if (failures.Any())
            throw new ValidationException(failures);
        return await next();
    }
}