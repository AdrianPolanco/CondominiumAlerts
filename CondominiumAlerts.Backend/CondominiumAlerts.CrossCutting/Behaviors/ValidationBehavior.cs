using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using CondominiumAlerts.CrossCutting.Results;
using FluentValidation;
using LightResults;
using MediatR;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace CondominiumAlerts.CrossCutting.Behaviors;

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest: ICommand<TResponse>
{
    
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Crear el contexto para la validación
        var context = new ValidationContext<TRequest>(request);

        // Validar todos los validadores disponibles
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Obtener todos los errores de validación
        var failures = validationResults
            .Where(r => r.Errors.Any()) // Filtrar los resultados con errores
            .SelectMany(r => r.Errors) // Aplanar los errores
            .ToList();

        // Si hay errores de validación, detener la ejecución y retornar los errores
        if (failures.Any())
        {
            // Convertir cada error de FluentValidation en un ValidationError
            var validationErrors = failures.Select(f => 
                new ValidationError(f.ErrorMessage, new Dictionary<string, object>
                {
                    { "PropertyName", f.PropertyName },
                    { "Code", f.ErrorCode },
                    {"AttemptedValue", f.AttemptedValue ?? "null"} 
                })
            ).ToList();

            var result = Result.Fail<TResponse>(validationErrors);

            // Retornar los errores de validación como un resultado fallido
            return (TResponse)(object)result;
        }

        // Si no hay errores de validación, continuar con la ejecución del siguiente handler
        return await next();
    }
}