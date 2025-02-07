using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using CondominiumAlerts.CrossCutting.Results;
using FluentValidation;
using LightResults;
using MediatR;
using Microsoft.Extensions.Logging;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace CondominiumAlerts.CrossCutting.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var validationResult = await _validators.First().ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var validationErrors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"Validation failed: {validationErrors}");
            
            return (TResponse)(object)Result.Fail<object>(validationErrors);
        }

        return await next();
    }
}