using MediatR;

namespace CondominiumAlerts.Domain.Events.Interfaces;

public interface IEvent : INotification
{
    DateTime Date { get; }
}