using System;
using System.Collections.Generic;
using Hse.CqrsWorkShop.Domain.Events;

namespace Hse.CqrsWorkShop.Domain
{
    public interface IAggregate
    {
        IEnumerable<IEvent> UncommitedEvents();
        void ClearUncommitedEvents();
        int Version { get; }
        Guid Id { get; }
        void ApplyEvent(IEvent @event);
    }
}