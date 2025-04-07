using System.Collections.Concurrent;

namespace CondominiumAlerts.Features.Features.Events;

public class ScheduledEventsService
{
    private readonly ConcurrentDictionary<Guid, DateTime> _startEvents = new();
    private readonly ConcurrentDictionary<Guid, DateTime> _endEvents = new();

    public void ScheduleEventStart(Guid eventId, DateTime startTime)
    {
        _startEvents[eventId] = startTime;
    }

    public void ScheduleEventEnd(Guid eventId, DateTime endTime)
    {
        _endEvents[eventId] = endTime;
    }

    public (Guid EventId, DateTime? ScheduledTime) GetNextEventToStart()
    {
        var currentTime = DateTime.UtcNow;
        var nextEvent = _startEvents
            .Where(kvp => kvp.Value <= currentTime)
            .OrderBy(kvp => kvp.Value)
            .FirstOrDefault();

        return nextEvent.Key != default ? (nextEvent.Key, nextEvent.Value) : (Guid.Empty, null);
    }

    public (Guid? EventId, DateTime? ScheduledTime) GetNextEventToEnd()
    {
        var currentTime = DateTime.UtcNow;
        var nextEvent = _endEvents
            .Where(kvp => kvp.Value <= currentTime)
            .OrderBy(kvp => kvp.Value)
            .FirstOrDefault();

        return nextEvent.Key != default ? (nextEvent.Key, nextEvent.Value) : (null, null);
    }

    public void RemoveProcessedStartEvent(Guid eventId)
    {
        _startEvents.TryRemove(eventId, out _);
    }

    public void RemoveProcessedEndEvent(Guid eventId)
    {
        _endEvents.TryRemove(eventId, out _);
    }
}