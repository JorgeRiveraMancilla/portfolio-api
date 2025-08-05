using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace API.Middleware
{
    public class ValidationBehavior<TRequest, TResponse>(
        IEnumerable<IValidator<TRequest>> validators
    ) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken
        )
        {
            if (_validators.Any())
            {
                ValidationContext<TRequest> context = new(request);
                ValidationResult[] validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken))
                );

                List<ValidationFailure> failures =
                [
                    .. validationResults.SelectMany(r => r.Errors).Where(f => f != null),
                ];

                if (failures.Count != 0)
                    throw new ValidationException(failures);
            }

            return await next(cancellationToken);
        }
    }
}
