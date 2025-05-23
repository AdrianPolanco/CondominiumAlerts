﻿using CondominiumAlerts.Domain.Events.Interfaces;
using MediatR;


namespace CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;

public interface IEventHandler<T> : INotificationHandler<T> where T : IEvent
{
    
}