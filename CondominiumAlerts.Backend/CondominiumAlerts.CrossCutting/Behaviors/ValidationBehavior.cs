using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using FluentValidation;
using MediatR;

namespace CondominiumAlerts.CrossCutting.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest: ICommand<TResponse>
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}