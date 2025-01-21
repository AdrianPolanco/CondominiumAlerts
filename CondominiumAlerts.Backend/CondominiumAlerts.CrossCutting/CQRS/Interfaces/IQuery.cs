using MediatR;

namespace CondominiumAlerts.CrossCutting.CQRS.Interfaces;

public interface IQuery<out TResponse> : IRequest<TResponse> where TResponse : notnull
{
    
}