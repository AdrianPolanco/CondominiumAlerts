using MediatR;

namespace CondominiumAlerts.CrossCutting.CQRS.Interfaces;

public interface ICommand : IRequest<Unit>
{
    
}

public interface ICommand<out TResponse> : IRequest<TResponse>
{
    
}