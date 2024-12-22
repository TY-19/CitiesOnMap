using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace CitiesOnMap.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var validationContext = new ValidationContext<TRequest>(request);
        ValidationResult[] validationResult = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(validationContext, cancellationToken)));
        List<ValidationFailure> failures = validationResult.Where(r => r.Errors.Count != 0)
            .SelectMany(r => r.Errors)
            .ToList();
        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}