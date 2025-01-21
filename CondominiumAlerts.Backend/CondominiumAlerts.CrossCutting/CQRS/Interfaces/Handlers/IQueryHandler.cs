using MediatR;

namespace CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;

public interface IQueryHandler <TQuery, TResponse>: IRequestHandler<TQuery, TResponse>
    where TQuery: IQuery<TResponse>
    where TResponse: notnull
{
    
}