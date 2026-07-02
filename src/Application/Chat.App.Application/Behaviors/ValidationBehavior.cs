using FluentValidation;
using MediatR;

namespace Chat.App.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, ct)));
            var failures = results.SelectMany(result => result.Errors).Where(error => error is not null).ToList();

            if (failures.Count != 0)
                throw new ValidationException(failures);
        }

        return await next(ct);
    }
}
